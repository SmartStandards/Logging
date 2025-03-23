using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Helper class for emitting messages into the (legacy) .NET System.Diagnostics.Trace concept
  /// </summary>
  internal class TraceBusFeed {

    private static Dictionary<string, TraceSource> _TraceSourcePerSourceContext = new Dictionary<string, TraceSource>();

    private static TraceSource GetTraceSourcePerSourceContext(string sourceContext) {

      TraceSource traceSource;

      lock (_TraceSourcePerSourceContext) {

        if (!_TraceSourcePerSourceContext.TryGetValue(sourceContext, out traceSource)) { // Lazily wire up the trace source...

          traceSource = new TraceSource(sourceContext);

          traceSource.Switch.Level = SourceLevels.All;

          traceSource.Listeners.Clear(); // Otherwise the default listener will be registered twice

          traceSource.Listeners.AddRange(Trace.Listeners); // Wire up all CURRENTLY existing trace listeners (they have to be initialized before!)

          _TraceSourcePerSourceContext[sourceContext] = traceSource;
        }
      }

      return traceSource;
    }

    internal static void EmitException(string sourceContext, string audience, int level, Exception ex) {
      int id = ExceptionSerializer.GetGenericIdFromException(ex);
      string serializedException = ex.Serialize();
      EmitMessage(sourceContext, audience, level, id, serializedException, new object[] { ex });
    }

    internal static void EmitMessage(string sourceContext, string audience, int level, int id, string messageTemplate, params object[] args) {

      TraceSource traceSource = GetTraceSourcePerSourceContext(sourceContext);

      if (traceSource is null) return;

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

        default: {
          eventType = TraceEventType.Verbose; // 16
          break;
        }

      }

      // Because we support named placeholders (like "Hello {person}") instead of old scool indexed place holders (like "Hello {0}")
      // we need to double brace the placeholders - otherwise there will be exceptions coming from the .net TraceEvent Method.

      string formatString = "(" + audience + ") " + messageTemplate?.Replace("{", "{{").Replace("}", "}}");

      traceSource.TraceEvent(eventType, id, formatString, args);
    }

  }

}
