using Logging.SmartStandards.Textualization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logging.SmartStandards.Transport {

  /// <summary>
  ///   Helper class for emitting messages into the (legacy) .NET System.Diagnostics.Trace concept
  /// </summary>
  public partial class TraceBusFeed { // v 1.0.0

    private MyCircularBuffer<QueuedEvent> _DebuggingLookBackBuffer;

    private static TraceBusFeed _Instance;

    public static TraceBusFeed Instance {
      get {

        if (_Instance == null) {
          _Instance = new TraceBusFeed();
        }

        return _Instance;
      }
    }

    public HashSet<string> IgnoredListeners { get; set; } = new HashSet<string>();

    private void FlushAndShutDownBuffer(TraceListener targetListener) {

      if (_DebuggingLookBackBuffer == null) return;

      lock (this) {

        _DebuggingLookBackBuffer.StopAutoFlush();

        foreach (QueuedEvent e in _DebuggingLookBackBuffer) {
          TraceEventCache eventCache = new TraceEventCache();
          InvokeListenerTraceEvent(targetListener, eventCache, e.EventType, e.SourceContext, e.KindId, e.MessageTemplate, e.Args);
        }

        _DebuggingLookBackBuffer = null;
      }
    }

    /// <summary>
    ///   Helper to invoke the TraceEvent() method of a TraceListener the right way.
    ///   Identify the right overload and respect thread safety.
    /// </summary>
    private void InvokeListenerTraceEvent(
      TraceListener targetListener, TraceEventCache eventCache, TraceEventType eventType, string sourceContext, int id, string format, params object[] args
    ) {
      if (targetListener.IsThreadSafe) {
        if (args == null) {
          targetListener.TraceEvent(eventCache, sourceContext, eventType, id, format);
        } else {
          targetListener.TraceEvent(eventCache, sourceContext, eventType, id, format, args);
        }
      } else {
        lock (targetListener) {
          if (args == null) {
            targetListener.TraceEvent(eventCache, sourceContext, eventType, id, format);
          } else {
            targetListener.TraceEvent(eventCache, sourceContext, eventType, id, format, args);
          }
        }
      }
    }

    private void ToAllRelevantListeners(TraceEventType eventType, string sourceContext, int id, string format, params object[] args) {

      TraceEventCache eventCache = new TraceEventCache(); // same instance for many listeners

      ForEachRelevantListener(
        (TraceListener listener) => {
          InvokeListenerTraceEvent(listener, eventCache, eventType, sourceContext, id, format, args);
        }
      );
    }

    public void EmitException(string audience, int level, string sourceContext, long sourceLineId, int kindId, Exception ex) {
      this.EmitMessage(audience, level, sourceContext, sourceLineId, kindId, ex.Message, new object[] { ex });
    }

    private DefaultTraceListener ForEachRelevantListener(Action<TraceListener> onProcessListener) {

      DefaultTraceListener foundDefaultTraceListener = null;

      foreach (TraceListener listener in Trace.Listeners) {

        if (this.IgnoredListeners.Contains(listener.Name)) continue;

        DefaultTraceListener defaultTraceListener = listener as DefaultTraceListener; // try cast

        if (defaultTraceListener != null) {

          bool isLogging = Debugger.IsLogging();

          foundDefaultTraceListener = defaultTraceListener;

          if (_DebuggingLookBackBuffer != null && isLogging) FlushAndShutDownBuffer(defaultTraceListener);

          // Is the default logger EmittingWorthy?
          if (!isLogging && String.IsNullOrWhiteSpace(defaultTraceListener.LogFileName)) continue;
        }

        onProcessListener.Invoke(listener);

      }
      return foundDefaultTraceListener;
    }

    /// <param name="level">
    ///   5 Critical
    ///   4 Error
    ///   3 Warning
    ///   2 Info
    ///   1 Debug
    ///   0 Trace
    /// </param>
    public void EmitMessage(
      string audience, int level, string sourceContext, long sourceLineId,
      int kindId, string messageTemplate, params object[] args
    ) {

      // Performance: Do not prepare a message that is never sent (or buffered)

      bool emittingWorthyListenersExist = false;

      DefaultTraceListener foundDefaultTraceListener = null;

      foundDefaultTraceListener = ForEachRelevantListener((TraceListener listener) => { emittingWorthyListenersExist = true; });

      if (!Debugger.IsLogging() && foundDefaultTraceListener != null && _DebuggingLookBackBuffer == null) {

        _DebuggingLookBackBuffer = new MyCircularBuffer<QueuedEvent>(1000);

        _DebuggingLookBackBuffer.StartAutoFlush(() => { if (Debugger.IsLogging()) FlushAndShutDownBuffer(foundDefaultTraceListener); }, 3000);
      }

      if (!emittingWorthyListenersExist && _DebuggingLookBackBuffer == null) return;

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (string.IsNullOrWhiteSpace(audience)) audience = "Dev";

      if (messageTemplate == null) messageTemplate = "";

      TraceEventType eventType;

      switch (level) {

        case 5: { // Critical (aka "Fatal")
          eventType = TraceEventType.Critical; // 1
          break;
        }

        case 4: { // Error
          eventType = TraceEventType.Error; // 2
          break;
        }

        case 3: { // Warning
          eventType = TraceEventType.Warning; // 4
          break;
        }

        case 2: { // Info
          eventType = TraceEventType.Information; // 8
          break;
        }

        case 1: { // Debug
          eventType = TraceEventType.Transfer; // 4096 - ' There is no "Debug" EventType => use something else
                                               // 0 "Trace" (aka "Verbose")
          break;
        }

        default: { // Trace
          eventType = TraceEventType.Verbose; // 16
          break;
        }

      }

      // Because we support named placeholders (like "Hello {person}") instead of old scool indexed place holders
      // (like "Hello {0}") we need to double brace the placeholders - otherwise there will be exceptions coming from
      // the .net TraceEvent Method.

      StringBuilder formatStringBuilder = new StringBuilder(messageTemplate.Length + 20);

      LogParaphRenderer.BuildParaphRightPart(formatStringBuilder, sourceLineId, audience, messageTemplate);

      formatStringBuilder.Replace("{", "{{").Replace("}", "}}");

      // actual emit

      ToAllRelevantListeners(eventType, sourceContext, kindId, formatStringBuilder.ToString(), args);

      if (_DebuggingLookBackBuffer != null) {
        lock (this) {
          _DebuggingLookBackBuffer.UnsafeEnqueue(new QueuedEvent(sourceContext, eventType, kindId, formatStringBuilder.ToString(), args));
        }
      }

    }

    internal class QueuedEvent {

      public string SourceContext;

      public TraceEventType EventType { get; set; }

      public int KindId { get; set; }

      public string MessageTemplate { get; set; }

      public object[] Args { get; set; }

      public QueuedEvent(string sourceContext, TraceEventType EventType, int kindId, string messageTemplate, object[] args) {
        SourceContext = sourceContext;
        this.EventType = EventType;
        this.KindId = kindId;
        this.MessageTemplate = messageTemplate;
        this.Args = args;
      }

    }

  }

}
