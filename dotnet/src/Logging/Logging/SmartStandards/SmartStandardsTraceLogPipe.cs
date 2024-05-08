using System;
using System.Diagnostics;
using System.Linq;

namespace Logging.SmartStandards {

  /// <summary>
  ///   [v 1.0.0]  Helper class to recieve log entries from System.Diagnostics.Trace. 
  ///   This is the counterpart to the default logging handler of LoggerBase.
  ///   Convention: Class name must end with "TraceLogPipe"!
  /// </summary>
  /// <remarks>
  ///   1. Due to limitations of System.Diagnostics.Trace you can use only one long-living instance per AppDomain.
  ///   
  ///   2. If you copy and paste this class (instead of referencing this library) make sure you change namespace and class name. 
  ///      Otherwise you'll run into naming collisionsif a 3rd party DLL references this library.
  /// </remarks>
  public class SmartStandardsTraceLogPipe : TraceListener {

    private static bool _Semaphore;

    private Action<string, int, int, string, string[]> _OnLog;

    private SmartStandardsTraceLogPipe() { // Constructor is private, because this must be a single instance
    }

    /// <param name="onLog">
    ///   Callback delegate which is called for each recieved log entry. Implement your actual logging code here.
    ///   Make sure this is done before initializing the TraceSource.
    /// </param>
    public static void Initialize(Action<string, int, int, string, string[]> onLog) {

      if (_Semaphore)
        throw new Exception("Do not initialize more than once!");

      var dummy = new SmartStandardsTraceLogPipe();

      dummy._OnLog = onLog;

      Trace.Listeners.Add(dummy); // Self-register to .net runtime

      // Remark: This is a one-way-ticket. Removing (even disposing) a listener is futile, because all TraceSources hold a reference.
      // You would have to iterate all existing TraceSources first an remove the listener there.
      // This is impossible due to the fact that there's no (clean) way of getting all existing TraceSources.
      // See: https://stackoverflow.com/questions/10581448/add-remove-tracelistener-to-all-tracesources
      // Therefore we must ensure a single instance of this class.

      _Semaphore = true;
    }

    public override void TraceEvent(
      TraceEventCache eventCache, string source, TraceEventType eventType, int eventId, string formatString, params object[] args
    ) {

      // Filter

      if (source != "Dev" && source != "Ins" && source != "Pro") return;

      // De-sanitize named placeholders

      string messageTemplate = formatString?.Replace("{{", "{").Replace("}}", "}");

      // Map EventType => LogLevel

      int level = this.EventTypeToLevel(eventType);

      // Push log entry to callback method

      string[] argsAsStrings = null;

      int argsMaxIndex = -1;

      if (args != null) argsMaxIndex = args.GetUpperBound(0);

      if (argsMaxIndex >= 0) {
        argsAsStrings = new string[argsMaxIndex + 1];
        for (int i = 0; i <= argsMaxIndex; i++) argsAsStrings[i] = args[i].ToString();
      }

      _OnLog.Invoke(source, level, eventId, messageTemplate, argsAsStrings);
    }

    private int EventTypeToLevel(TraceEventType eventType) {
      switch (eventType) {
        case TraceEventType.Critical: { // (aka "Fatal")
          return 5;
        }
        case TraceEventType.Error: {
          return 4;
        }
        case TraceEventType.Warning: {
          return 3;
        }
        case TraceEventType.Information: {
          return 2;
        }
        case TraceEventType.Transfer: { // There is no "Debug" EventType => use something else
          return 1; // "Debug"
        }

        default: {
          return 0; // "Trace" (aka "Verbose")
        }
      }
    }

    public override void Write(string message) {
      // Not needed
    }

    public override void WriteLine(string message) {
      // Not needed
    }

  }

}