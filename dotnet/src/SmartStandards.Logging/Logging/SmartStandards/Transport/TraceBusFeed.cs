using Logging.SmartStandards.Textualization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logging.SmartStandards.Transport {

  /// <summary>
  ///   Helper class for emitting messages into the (legacy) .NET System.Diagnostics.Trace concept
  /// </summary>
  public partial class TraceBusFeed { // v 2.0.0

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

    public List<string> RawModeListeners { get; set; } = new List<string>() { "SmartStandards395316649" };

    public HashSet<string> IgnoredListeners { get; set; } = new HashSet<string>();

    public bool AutoFlush { get; set; } = true;

    private void FlushAndShutDownBuffer(TraceListener targetListener) {

      if (_DebuggingLookBackBuffer == null) return;

      lock (this) {

        _DebuggingLookBackBuffer.StopAutoFlush();

        foreach (QueuedEvent e in _DebuggingLookBackBuffer) {
          this.InvokeListener(targetListener, e.Audience, e.Level, e.SourceContext, e.SourceLineId, e.EventKindId, e.MessageTemplate, e.Args);
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
          if (this.AutoFlush) targetListener.Flush();
        } else {
          targetListener.TraceEvent(eventCache, sourceContext, eventType, id, format, args);
          if (this.AutoFlush) targetListener.Flush();
        }
      } else {
        lock (targetListener) {
          if (args == null) {
            targetListener.TraceEvent(eventCache, sourceContext, eventType, id, format);
            if (this.AutoFlush) targetListener.Flush();
          } else {
            targetListener.TraceEvent(eventCache, sourceContext, eventType, id, format, args);
            if (this.AutoFlush) targetListener.Flush();
          }
        }
      }
    }

    private void InvokeListenerWriteLine(TraceListener targetListener, string text) {
      if (targetListener.IsThreadSafe) {
        targetListener.WriteLine(text);
        if (this.AutoFlush) targetListener.Flush();
      } else {
        lock (targetListener) {
          targetListener.WriteLine(text);
        }
      }
    }

    /// <param name="targetListener"> If null, invoke all listeners. </param>
    private void InvokeListener(
      TraceListener targetListener,
      string audience, int level, string sourceContext, long sourceLineId,
      int eventKindId, string messageTemplate, params object[] args
    ) {

      StringBuilder logParaphBuilder = null;

      TraceEventCache eventCache = null;
      TraceEventType eventType = TraceEventType.Verbose;
      StringBuilder rightPartOnlyBuilder = null;

      // Lazily create stuff that is needed only in case we need to pass the log event as presentation-ready

      Action onPreparePresentationReadyStuff = () => {
        if (logParaphBuilder == null) {
          logParaphBuilder = new StringBuilder(messageTemplate.Length + 20);
          if (args.Length > 0 && args[0] is Exception) {
            ExceptionRenderer.AppendToStringBuilder(logParaphBuilder, (Exception)args[0]);
          } else {
            LogParaphRenderer.BuildParaphResolved(
              logParaphBuilder, audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args
            );
          }
        }
      };

      // Lazily create stuff that is needed only in case we need to pass the log event as trace event

      Action onPrepareTraceEventStuff = () => {
        if (eventCache == null) {
          eventCache = new TraceEventCache(); // same instance for many listeners
          eventType = LevelToTraceEventType(level);

          rightPartOnlyBuilder = new StringBuilder(messageTemplate.Length + 20);
          LogParaphRenderer.BuildParaphRightPart(rightPartOnlyBuilder, sourceLineId, audience, messageTemplate);
          rightPartOnlyBuilder.Replace("{", "{{").Replace("}", "}}");
        }
      };

      // This is the inner snippet, actually invoking a listener

      Action<TraceListener> onInvokeListener = (TraceListener listener) => {
        bool useRawMode = this.RawModeListeners.Contains(listener.Name);
        if (useRawMode) {
          onPrepareTraceEventStuff.Invoke();
          this.InvokeListenerTraceEvent(listener, eventCache, eventType, sourceContext, eventKindId, rightPartOnlyBuilder.ToString(), args);
        } else {
          onPreparePresentationReadyStuff.Invoke();
          this.InvokeListenerWriteLine(listener, logParaphBuilder.ToString());
        }
      };

      // this is the outer loop

      if (targetListener != null) { // invoke only one specific listener

        onInvokeListener(targetListener);

      } else { // no specific listener -> invok all (relevant) listeners

        this.ForEachRelevantListener(
          (TraceListener listener) => { onInvokeListener(listener); }
        );
      }
    }

    public void EmitException(string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex) {
      this.EmitMessage(audience, level, sourceContext, sourceLineId, eventKindId, ex.Message, new object[] { ex });
    }

    private DefaultTraceListener ForEachRelevantListener(Action<TraceListener> onProcessListener) {

      DefaultTraceListener foundDefaultTraceListener = null;

      foreach (TraceListener listener in Trace.Listeners) {

        if (this.IgnoredListeners.Contains(listener.Name)) continue;

        DefaultTraceListener defaultTraceListener = listener as DefaultTraceListener; // try cast

        if (defaultTraceListener != null) {

          bool isLogging = Debugger.IsLogging();

          foundDefaultTraceListener = defaultTraceListener;

          if (_DebuggingLookBackBuffer != null && isLogging) this.FlushAndShutDownBuffer(defaultTraceListener);

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
      int eventKindId, string messageTemplate, params object[] args
    ) {

      // Performance: Do not prepare a message that is never sent (or buffered)

      bool emittingWorthyListenersExist = false;

      DefaultTraceListener foundDefaultTraceListener = null;

      foundDefaultTraceListener = this.ForEachRelevantListener((TraceListener listener) => { emittingWorthyListenersExist = true; });

      if (!Debugger.IsLogging() && foundDefaultTraceListener != null && _DebuggingLookBackBuffer == null) {

        _DebuggingLookBackBuffer = new MyCircularBuffer<QueuedEvent>(1000);

        _DebuggingLookBackBuffer.StartAutoFlush(() => { if (Debugger.IsLogging()) this.FlushAndShutDownBuffer(foundDefaultTraceListener); }, 3000);
      }

      if (!emittingWorthyListenersExist && _DebuggingLookBackBuffer == null) return;

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (string.IsNullOrWhiteSpace(audience)) audience = "Dev";

      if (messageTemplate == null) messageTemplate = "";

      // actual emit

      this.InvokeListener(null, audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);

      if (_DebuggingLookBackBuffer != null) {
        lock (this) {
          _DebuggingLookBackBuffer.UnsafeEnqueue(new QueuedEvent(audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args));
        }
      }

    }

    private static TraceEventType LevelToTraceEventType(int level) {
      switch (level) {
        case 5: return TraceEventType.Critical; // 1 Critical (aka "Fatal")
        case 4: return TraceEventType.Error; // 2
        case 3: return TraceEventType.Warning; // 4
        case 2: return TraceEventType.Information; // 8
        case 1: return TraceEventType.Transfer; // 4096 - ' There is no "Debug" EventType => use something else                
        default: return TraceEventType.Verbose; // 16 "Trace"
      }
    }

  }

  internal class QueuedEvent {

    public string Audience { get; set; }

    public int Level { get; set; }

    public string SourceContext { get; set; }

    public long SourceLineId { get; set; }

    public int EventKindId { get; set; }

    public string MessageTemplate { get; set; }

    public object[] Args { get; set; }

    public QueuedEvent(
      string audience, int level, string sourceContext, long sourceLineId,
      int eventKindId, string messageTemplate, params object[] args
    ) {
      this.Audience = audience;
      this.Level = level;
      this.SourceContext = sourceContext;
      this.SourceLineId = sourceLineId;
      this.EventKindId = eventKindId;
      this.MessageTemplate = messageTemplate;
      this.Args = args;
    }

  }

}
