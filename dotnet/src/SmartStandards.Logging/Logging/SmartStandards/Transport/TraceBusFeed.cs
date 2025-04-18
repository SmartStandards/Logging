using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging.SmartStandards.Textualization;

namespace Logging.SmartStandards.Transport {

  /// <summary>
  ///   Helper class for emitting messages into the (legacy) .NET System.Diagnostics.Trace concept
  /// </summary>
  public partial class TraceBusFeed {

    private Dictionary<string, TraceSource> _TraceSourcePerSourceContext = new Dictionary<string, TraceSource>();

    private MyCircularBuffer<QueuedEvent> _EarlyPhaseBuffer;

    private readonly object _BufferManipulating = new object();

    private static TraceBusFeed _Instance;

    private bool _PatienceExercised;

    public static TraceBusFeed Instance {
      get {

        if (_Instance == null) {
          _Instance = new TraceBusFeed();
          _Instance._EarlyPhaseBuffer = new MyCircularBuffer<QueuedEvent>(1000);
        }

        return _Instance;
      }
    }

    private bool ListenersAvailable {
      get {

        if (Trace.Listeners.Count == 0) return false;

        if (Trace.Listeners.Count == 1) {
          string listenerName = Trace.Listeners[0].Name;
          if (listenerName == "Default") return false;
          if (this.IgnoredListeners.Contains(listenerName)) return false;
        }

        return true;
      }
    }

    public HashSet<string> IgnoredListeners { get; set; } = new HashSet<string>();

    private TraceSource GetTraceSourcePerSourceContext(string sourceContext) {

      lock (_TraceSourcePerSourceContext) {

        TraceSource traceSource = null;

        // get or (lazily) create TraceSource

        if (!_TraceSourcePerSourceContext.TryGetValue(sourceContext, out traceSource)) {

          // Optimization: Do not instantiate a TraceSource if there are no listeners:
          if (!this.ListenersAvailable) return null;

          // Instantiate a TraceSource:
          traceSource = new TraceSource(sourceContext);
          traceSource.Switch.Level = SourceLevels.All;

          _TraceSourcePerSourceContext[sourceContext] = traceSource;

          // when a new trace source was created => always keep all listeners of all TraceSource in sync:

          this.RewireAllSourcesAndListeners();
        }

        return traceSource;
      } // lock
    }

    private void FlushAndShutDownBuffer() {

      lock (_TraceSourcePerSourceContext) {

        if (_EarlyPhaseBuffer == null) return;

        MyCircularBuffer<QueuedEvent> earlyPhaseBuffer = _EarlyPhaseBuffer;
        _EarlyPhaseBuffer = null; // set field to null to avoid unwanted recursion

        foreach (QueuedEvent e in earlyPhaseBuffer) {
          TraceSource traceSource = this.GetTraceSourcePerSourceContext(e.SourceContext);
          traceSource?.TraceEvent(e.EventType, e.KindId, e.MessageTemplate, e.Args);
        }
      }
    }

    private void RewireAllSourcesAndListeners() {

      TraceSource firstTraceSource = null;

      bool awaitedListenerFound = false;

      foreach (KeyValuePair<string, TraceSource> namedTraceSource in _TraceSourcePerSourceContext) {

        if (firstTraceSource == null) {

          firstTraceSource = namedTraceSource.Value;

          firstTraceSource.Listeners.Clear();

          // Cherry-pick available listeners => store into the first TraceSource (representative of all others)

          awaitedListenerFound = this.CaptureListenersInto(firstTraceSource);

        } else { // subsequent trace sources get the same listeners as the first one
          namedTraceSource.Value.Listeners.Clear();
          namedTraceSource.Value.Listeners.AddRange(firstTraceSource.Listeners);
        }

      } // next namedTraceSource

      if (_EarlyPhaseBuffer != null && awaitedListenerFound) {
        this.FlushAndShutDownBuffer();
      }

    }

    private bool CaptureListenersInto(TraceSource targetTraceSource) {

      bool awaitedListenerFound = false;

      foreach (TraceListener listener in Trace.Listeners) {

        if (listener.Name == "Default") continue; // The .NET Default listener is a major performance hit => do not support.

        if (this.IgnoredListeners.Contains(listener.Name)) continue;

        targetTraceSource?.Listeners.Add(listener);

        if (listener.Name == "SmartStandards395316649") awaitedListenerFound = true;
      }

      return awaitedListenerFound;
    }

    public void EmitException(string audience, int level, string sourceContext, long sourceLineId, int kindId, Exception ex) {
      this.EmitMessage(audience, level, sourceContext, sourceLineId, kindId, ex.Message, new object[] { ex });
    }

    private void KillBufferAfterGracePeriod() {

      Thread.Sleep(10000);

      lock (_TraceSourcePerSourceContext) {

        bool awaitedListenerFound = this.CaptureListenersInto(null);

        if (awaitedListenerFound) this.FlushAndShutDownBuffer();

        _EarlyPhaseBuffer = null;
      }
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

      if (!_PatienceExercised) {
        _PatienceExercised = true;
        Task.Run(this.KillBufferAfterGracePeriod);
      }

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      TraceSource traceSource = this.GetTraceSourcePerSourceContext(sourceContext);

      if (traceSource is null && _EarlyPhaseBuffer == null) return;

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

      traceSource?.TraceEvent(eventType, kindId, formatStringBuilder.ToString(), args);

      if (_EarlyPhaseBuffer != null) {
        _EarlyPhaseBuffer.SafeEnqueue(new QueuedEvent(sourceContext, eventType, kindId, formatStringBuilder.ToString(), args));
      }

    }

    private class QueuedEvent {

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
