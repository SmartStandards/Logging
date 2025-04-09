using Logging.SmartStandards.Textualization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logging.SmartStandards.Transport {

  /// <summary>
  ///   Helper class for emitting messages into the (legacy) .NET System.Diagnostics.Trace concept
  /// </summary>
  public partial class TraceBusFeed {

    private Dictionary<string, TraceSource> _TraceSourcePerSourceContext = new Dictionary<string, TraceSource>();

    private Dictionary<string, MyCircularBuffer<QueuedEvent>> _RequiredListenersMessageQueue;

    /// <summary>
    ///   Emit textualized exceptions (as message) to the TraceBus (instead of the original exception as arg).
    /// </summary>
    public bool ExceptionsTextualizedToggle { get; set; }

    public HashSet<string> RequiredListeners { get; set; } = new HashSet<string>();

    public HashSet<string> IgnoredListeners { get; set; } = new HashSet<string>();

    private TraceSource GetTraceSourcePerSourceContext(string sourceContext) {

      TraceSource traceSource;

      lock (_TraceSourcePerSourceContext) {

        if (!_TraceSourcePerSourceContext.TryGetValue(sourceContext, out traceSource)) {

          // Lazily wire up the trace source - must be done as late as possible
          // (after listeners have registered themselves)

          traceSource = new TraceSource(sourceContext);

          traceSource.Switch.Level = SourceLevels.All;

          WireUp(traceSource);

          _TraceSourcePerSourceContext[sourceContext] = traceSource;
        }
      }

      EnsureRequiredListeners(traceSource);

      return traceSource;
    }

    private void EnsureRequiredListeners(TraceSource traceSource) {

      if (RequiredListeners.Count == 0) return;

      foreach (TraceListener listener in Trace.Listeners) {

        if (this.RequiredListeners.Contains(listener.Name)) {

          this.RequiredListeners.Remove(listener.Name);

          if (!traceSource.Listeners.Contains(listener)) traceSource.Listeners.Add(listener);

          MyCircularBuffer<QueuedEvent> buffer;

          if (_RequiredListenersMessageQueue.TryGetValue(listener.Name, out buffer)) {

            _RequiredListenersMessageQueue.Remove(listener.Name);

            foreach (QueuedEvent e in buffer) {
              traceSource.TraceEvent(e.EventType, e.KindId, e.MessageTemplate, e.Args);
            }

          }

        }
      }
    }

    private void WireUp(TraceSource traceSource) {

      traceSource.Listeners.Clear(); // Boilerplate: Always remove the ominous "Default" TraceListener

      foreach (TraceListener listener in Trace.Listeners) {

        if (listener.Name == "Default") continue; // The .NET Default listener is a major performance hit => do not support.

        if (this.RequiredListeners.Contains(listener.Name)) this.RequiredListeners.Remove(listener.Name);

        if (this.IgnoredListeners.Contains(listener.Name)) continue;

        traceSource.Listeners.Add(listener);

      }
    }

    public void EmitException(string audience, int level, string sourceContext, long sourceLineId, int kindId, Exception ex) {

      string renderedException = "";
      object[] args;

      if (this.ExceptionsTextualizedToggle) {
        renderedException = ExceptionRenderer.Render(ex);
        args = Array.Empty<object>();
      } else {
        args = new object[] { ex };
      }

      this.EmitMessage(audience, level, sourceContext, sourceLineId, kindId, renderedException, args);
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

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      TraceSource traceSource = this.GetTraceSourcePerSourceContext(sourceContext);

      if (traceSource is null) return;

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

      traceSource.TraceEvent(eventType, kindId, formatStringBuilder.ToString(), args);

      // queue

      if (this.RequiredListeners.Count > 0) {

        if (_RequiredListenersMessageQueue == null) _RequiredListenersMessageQueue = new Dictionary<string, MyCircularBuffer<QueuedEvent>>();

        foreach (string listenerName in this.RequiredListeners) {

          MyCircularBuffer<QueuedEvent> b;

          if (!_RequiredListenersMessageQueue.TryGetValue(listenerName, out b)) {
            b = new MyCircularBuffer<QueuedEvent>(1000);
            _RequiredListenersMessageQueue.Add(listenerName, b);
          }

          b.SafeEnqueue(new QueuedEvent(eventType, kindId, formatStringBuilder.ToString(), args));

        }
      }

    }

    private class QueuedEvent {

      public TraceEventType EventType { get; set; }

      public int KindId { get; set; }

      public string MessageTemplate { get; set; }

      public object[] Args { get; set; }

      public QueuedEvent(TraceEventType EventType, int kindId, string messageTemplate, object[] args) {
        this.EventType = EventType;
        this.KindId = kindId;
        this.MessageTemplate = messageTemplate;
        this.Args = args;
      }

    }

  }

}
