﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Helper class for emitting messages into the (legacy) .NET System.Diagnostics.Trace concept
  /// </summary>
  public class TraceBusFeed {

    public List<object> IgnoredListeners { get; set; } = new List<object>();

    private Dictionary<string, TraceSource> _TraceSourcePerSourceContext = new Dictionary<string, TraceSource>();

    private TraceSource GetTraceSourcePerSourceContext(string sourceContext) {

      TraceSource traceSource;

      lock (_TraceSourcePerSourceContext) {

        if (!_TraceSourcePerSourceContext.TryGetValue(sourceContext, out traceSource)) {

          // Lazily wire up the trace source - must be done as late as possible
          // (after listeners have registered themselves)

          traceSource = new TraceSource(sourceContext);

          traceSource.Switch.Level = SourceLevels.All;

          traceSource.Listeners.Clear(); // Otherwise the default listener will be registered twice

          // Wire up all CURRENTLY existing trace listeners (they have to be initialized before!)

          foreach (TraceListener listener in Trace.Listeners) {
            if (!IgnoredListeners.Contains(listener)) traceSource.Listeners.Add(listener);
          }

          _TraceSourcePerSourceContext[sourceContext] = traceSource;
        }
      }

      return traceSource;
    }

    public void EmitException(string audience, int level, string sourceContext, long sourceLineId, Exception ex) {
      int eventId = ExceptionSerializer.GetGenericIdFromException(ex);//todo nicht hier, beim Aufrufer!
      string serializedException = ex.Serialize();
      EmitMessage(audience, level, sourceContext, sourceLineId, eventId, serializedException, new object[] { ex });
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
      int eventId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      TraceSource traceSource = GetTraceSourcePerSourceContext(sourceContext);

      if (traceSource is null) return;

      if (string.IsNullOrWhiteSpace(audience)) audience = "Unk";

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

      string formatString = " " + sourceLineId.ToString() + " [" + audience + "]: " + messageTemplate?.Replace("{", "{{").Replace("}", "}}");

      traceSource.TraceEvent(eventType, eventId, formatString, args);
    }

  }

}
