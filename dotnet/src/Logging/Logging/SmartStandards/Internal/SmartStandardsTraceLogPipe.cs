[assembly: InternalsVisibleTo("Logging.Tests")]

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
  public sealed class SmartStandardsTraceLogPipe
 : TraceListener {

    private static SmartStandardsTraceLogPipe _Instance;

    private LogMethod _OnLog;

    //used to suppress output (likee it would be terminated)
    private bool _IsPseudoTerminated = false;

    private SmartStandardsTraceLogPipe() { // Constructor is private, because this must be a single instance
    }

    public static bool IsInitialized {
      get {
        return (_Instance != null);
      }
    }

    /// <summary>
    ///   Registers the pipe as trace listener, allowing trace sources to wire up to the pipe.
    ///   Wires up your actual logging sink by registering a callback delegate.
    /// </summary>
    /// <param name="onLog">
    ///   Callback delegate which is called for each recieved log entry. 
    ///   Signature: onLog(string channelName, int level, int id, string messageTemplate, object[] args)
    ///   Implement your actual logging sink here.
    /// </param>
    /// <remarks>
    ///   Order dependency: This has to be done before initializing any trace sources 
    ///   (= before any usage of a logger, because the loggers implicitely wire up their internal trace source on first usage).
    /// </remarks>
    public static void Initialize(LogMethod onLog) {

      if (IsInitialized) {
        if (_Instance._IsPseudoTerminated) {
          _Instance._IsPseudoTerminated = false;
          return;
        } else {
          throw new Exception("Do not initialize more than once!");
        }
      }
      _Instance = new SmartStandardsTraceLogPipe {
        _OnLog = onLog
      };

      Trace.Listeners.Add(_Instance); // Self-register to .net runtime

      // Remark: This is a one-way-ticket. Removing (even disposing) a listener is futile, because all TraceSources hold a reference.
      // You would have to iterate all existing TraceSources first an remove the listener there.
      // This is impossible due to the fact that there's no (clean) way of getting all existing TraceSources.
      // See: https://stackoverflow.com/questions/10581448/add-remove-tracelistener-to-all-tracesources
      // Therefore we must ensure a single instance of this class.

    }

    public static void Terminate() {
      if (IsInitialized) {
        _Instance._IsPseudoTerminated = true;
      }
    }

    /// <summary>
    ///   Registers the pipe as trace listener, allowing trace sources to wire up to the pipe.
    ///   Wires into the DevLogger, InfrastructureLogger or ProtocolLogger,
    ///   but only if their LoggingMethod's are redirected.
    /// </summary>
    public static void InitializeAsLoggerInput() {
      Initialize(
        (string audienceToken, int level, int id, string messageTemplate, object[] messageArgs) => {

          Exception ex = null;

          if (messageArgs != null && messageArgs.Length > 0) {
            if (messageArgs[0] is Exception) {
              ex = (Exception)messageArgs[0];
            }
            if (messageArgs[messageArgs.Length - 1].Equals(DevLogger.MirrorMarker)) {
              return;
            }
          }

          if (audienceToken == DevLogger.ChannelName && DevLogger.AwaitsInputFromTracing) {
            //je nach bedarf in die normale oder in die exception-log methode umleiten
            if (ex == null)
              DevLogger.InternalLogMethod.Invoke(DevLogger.ChannelName, true, level, id, messageTemplate, messageArgs);
            else
              DevLogger.InternalExceptionLogMethod.Invoke(DevLogger.ChannelName, true, level, id, ex);

          } else if (audienceToken == InfrastructureLogger.ChannelName && InfrastructureLogger.AwaitsInputFromTracing) {
            //je nach bedarf in die normale oder in die exception-log methode umleiten
            if (ex == null)
              InfrastructureLogger.InternalLogMethod.Invoke(InfrastructureLogger.ChannelName, true, level, id, messageTemplate, messageArgs);
            else
              InfrastructureLogger.InternalExceptionLogMethod.Invoke(InfrastructureLogger.ChannelName, true, level, id, ex);

          } else if (audienceToken == ProtocolLogger.ChannelName && ProtocolLogger.AwaitsInputFromTracing) {
            //je nach bedarf in die normale oder in die exception-log methode umleiten
            if (ex == null)
              ProtocolLogger.InternalLogMethod.Invoke(ProtocolLogger.ChannelName, true, level, id, messageTemplate, messageArgs);
            else
              ProtocolLogger.InternalExceptionLogMethod.Invoke(ProtocolLogger.ChannelName, true, level, id, ex);

          }
        }
      );
    }

    public override void TraceEvent(
      TraceEventCache eventCache, string audienceToken, TraceEventType eventType, int eventId, string formatString, params object[] args
    ) {

      if (_IsPseudoTerminated) return;

      // Filter

      if (audienceToken != DevLogger.ChannelName && audienceToken != InfrastructureLogger.ChannelName && audienceToken != ProtocolLogger.ChannelName) return;

      // De-sanitize named placeholders

      string messageTemplate = formatString?.Replace("{{", "{").Replace("}}", "}");

      // Map EventType => LogLevel

      int level = 0; // Default: "Trace" (aka "Verbose")

      switch (eventType) {
        case TraceEventType.Critical: level = 5; break; // (aka "Fatal")
        case TraceEventType.Error: level = 4; break;
        case TraceEventType.Warning: level = 3; break;
        case TraceEventType.Information: level = 2; break;
        case TraceEventType.Transfer: level = 1; break; // There is no "Debug" EventType => (mis)use something else        
      }

      // Push log entry to callback method

      _OnLog.Invoke(audienceToken, level, eventId, messageTemplate, args);
    }

    public override void Write(string message) { } // Not needed

    public override void WriteLine(string message) { }// Not needed

  }

}
