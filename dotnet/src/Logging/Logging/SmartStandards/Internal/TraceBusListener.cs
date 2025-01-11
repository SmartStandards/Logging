using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Logging.Tests")]

namespace Logging.SmartStandards {

  /// <summary>
  ///   [v 1.0.0]  Helper class to recieve log entries from System.Diagnostics.Trace. 
  /// </summary>
  /// <remarks>
  ///   1. Due to limitations of System.Diagnostics.Trace you can use only one long-living instance per AppDomain.
  ///   
  ///   2. If you copy and paste this class (instead of referencing this library) make sure you change namespace and class name. 
  ///      Otherwise you'll run into naming collisionsif a 3rd party DLL references this library.
  /// </remarks>
  public class TraceBusListener : TraceListener {

    public delegate bool FilterIncomingTraceEventDelegate(string sourceName, string formatString, out string audienceToken);

    public delegate void PassThroughDelegate(string sourceContext, string audience, int level, int id, string messageTemplate, object[] args);

    private static TraceBusListener _Instance;

    protected TraceBusListener() { // Constructor is private, because this must be a single instance
    }

    public static FilterIncomingTraceEventDelegate OnFilterIncomingTraceEvent { get; set; }

    public static PassThroughDelegate OnPassThrough { get; set; }

    public static bool IsInitialized {
      get {
        return (_Instance != null);
      }
    }

    /// <summary>
    ///   Registers the pipe as trace listener, allowing trace sources to wire up to the pipe.
    ///   Wires up your actual logging sink by registering a callback delegate.
    /// </summary>
    /// <param name="onPassThrough">
    ///   Callback delegate which is called for each recieved log entry. 
    ///   Signature: onLog(string channelName, int level, int id, string messageTemplate, object[] args)
    ///   Implement your actual logging sink here.
    /// </param>
    /// <remarks>
    ///   Order dependency: This has to be done before initializing any trace sources 
    ///   (= before any usage of a logger, because the loggers implicitely wire up their internal trace source on first usage).
    /// </remarks>
    public static void Initialize(PassThroughDelegate onPassThrough) {

      if (IsInitialized) {
        throw new Exception("Do not initialize more than once!");
      }

      _Instance = new TraceBusListener();

      OnPassThrough = onPassThrough;

      Trace.Listeners.Add(_Instance); // Self-register to .net runtime

      // Remark: This is a one-way-ticket. Removing (even disposing) a listener is futile, because all TraceSources hold a reference.
      // You would have to iterate all existing TraceSources first an remove the listener there.
      // This is impossible due to the fact that there's no (clean) way of getting all existing TraceSources.
      // See: https://stackoverflow.com/questions/10581448/add-remove-tracelistener-to-all-tracesources
      // Therefore we must ensure a single instance of this class.

    }

    public override void TraceEvent(
      TraceEventCache eventCache, string sourceName, TraceEventType eventType, int id, string formatString, params object[] args
    ) {

      if (OnPassThrough == null) {
        return;
      }

      string audienceToken = null;

      // Filtering & AudienceToken-Parsing:

      if (OnFilterIncomingTraceEvent != null) {
        OnFilterIncomingTraceEvent.Invoke(sourceName, formatString, out audienceToken);
      }

      // Unescape named placeholders

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

      OnPassThrough.Invoke(sourceName, audienceToken, level, id, messageTemplate, args);
    }

    public override void Write(string message) { } // Not needed

    public override void WriteLine(string message) { }// Not needed

  }

}
