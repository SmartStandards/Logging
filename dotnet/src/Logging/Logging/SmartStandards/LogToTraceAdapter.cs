using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logging.SmartStandards {

  internal class LogToTraceAdapter {

    private static Dictionary<string, TraceSource> _TraceSourcePerAudienceToken;

    private static TraceSource GetTraceSourcePerAudienceName(string audience) {

      if (_TraceSourcePerAudienceToken is null) _TraceSourcePerAudienceToken = new Dictionary<string, TraceSource>();

      TraceSource traceSource;

      if (!_TraceSourcePerAudienceToken.TryGetValue(audience, out traceSource)) { // Lazily wire up the trace source...

        // ... but ensure the desired listener was initialized before:

        bool found = false;

        foreach (TraceListener l in Trace.Listeners) {
          if (l.GetType().Name.EndsWith("TraceLogPipe")) { // convention!
            found = true;
            break;
          }
        }

        if (!found) return null;

        // actual wire-up

        traceSource = new TraceSource(audience);
        traceSource.Switch.Level = SourceLevels.All;
        traceSource.Listeners.Clear(); // Otherwise the default listener will be registered twice
        traceSource.Listeners.AddRange(Trace.Listeners); // Wire up all CURRENTLY existing trace listeners (they have to be initialized before!)

        _TraceSourcePerAudienceToken[audience] = traceSource;

      }
      return traceSource;
    }

    internal static void LogToTrace(string audience, int level, int id, string messageTemplate, params object[] args) {

      TraceSource traceSource = GetTraceSourcePerAudienceName(audience);

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

      // Because we support named placeholders (like "Hello {audience}") instead of old scool indexed place holders (like "Hello {0}")
      // we need to double brace the placeholders - otherwise there will be exceptions coming from the .net TraceEvent Method.

      string formatString = messageTemplate?.Replace("{", "{{").Replace("}", "}}");

      traceSource.TraceEvent(eventType, id, formatString, args);
    }

  }

}
