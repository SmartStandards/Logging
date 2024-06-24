using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Logging.SmartStandards {

  public sealed class UnitTestTraceListener : TraceListener {

    private static UnitTestTraceListener _Instance;

    private UnitTestTraceListener() { // Constructor is private, because this must be a single instance
    }

    //used to suppress output (likee it would be terminated)
    private bool _IsPseudoTerminated = false;

    public static bool IsInitialized {
      get {
        return (_Instance != null);
      }
    }

    public static void EnsureIsInitialized() {
      if (IsInitialized) {
        _Instance._IsPseudoTerminated = false;
        return;
      }
      _Instance = new UnitTestTraceListener();
      Trace.Listeners.Add(_Instance); // Self-register to .net runtime
    }

    public static void Terminate() {
      if (IsInitialized) {
        _Instance._IsPseudoTerminated = true;
      }
    }

    public static string LastSource { get; private set; }
    public static int LastLevel { get; private set; }
    public static int LastEventId { get; private set; }
    public static string LastMessageTemplate { get; private set; }
    public static object[] LastArgs { get; private set; }

    public override void TraceEvent(
      TraceEventCache eventCache, string source, TraceEventType eventType, int eventId, string formatString, params object[] args
    ) {

      if (_IsPseudoTerminated) return;

      if (source != DevLogger.ChannelName && source != InfrastructureLogger.ChannelName && source != ProtocolLogger.ChannelName) return;
      LastSource = source;
      LastLevel = 0; // Default: "Trace" (aka "Verbose")
      switch (eventType) {
        case TraceEventType.Critical: LastLevel = 5; break; // (aka "Fatal")
        case TraceEventType.Error: LastLevel = 4; break;
        case TraceEventType.Warning: LastLevel = 3; break;
        case TraceEventType.Information: LastLevel = 2; break;
        case TraceEventType.Transfer: LastLevel = 1; break; // There is no "Debug" EventType => (mis)use something else        
      }
      LastEventId = eventId;
      LastMessageTemplate = formatString?.Replace("{{", "{").Replace("}}", "}");
      LastArgs = args;
    }

    public override void Write(string message) { } // Not needed

    public override void WriteLine(string message) { }// Not needed

  }

}
