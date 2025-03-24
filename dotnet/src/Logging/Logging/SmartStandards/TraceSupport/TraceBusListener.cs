using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Logging.Tests")]

namespace Logging.SmartStandards {

  /// <summary>
  ///   Helper class to recieve log entries from System.Diagnostics.Trace. 
  /// </summary>
  /// <remarks>
  ///   1. Due to limitations of System.Diagnostics.Trace you can use only one long-living instance per AppDomain.
  ///   
  ///   2. If you copy and paste this class (instead of referencing this library) be sure to change namespace and 
  ///      class name. Otherwise you'll run into naming collisionsif a 3rd party DLL references this library.
  /// </remarks>
  public class TraceBusListener : TraceListener {

    /// <summary>
    ///   3rd party trace sources may not provide an audience. 
    ///   Therefore you can map their source name to a target audience.
    /// </summary>
    public Dictionary<string, string> TraceSourcesToAudienceMapping { get; set; } = new Dictionary<string, string>();

    public delegate bool FilterIncomingTraceEventDelegate(int eventType, string sourceName, string formatString);

    public delegate void OnLogEventReceivedDelegate(string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, object[] args);

    protected TraceBusListener() {
      // Constructor is private, because this must be a single instance
    }

    public OnLogEventReceivedDelegate OnLogEventReceived { get; set; }

    /// <summary>
    ///   Registers the pipe as trace listener, allowing trace sources to wire up to the pipe.
    ///   Wires up your actual logging sink by registering a callback delegate.
    /// </summary>
    /// <param name="onLogEventReceived">
    ///   Callback delegate which is called for each recieved log entry. 
    ///   Signature: onLog(string channelName, int level, int id, string messageTemplate, object[] args)
    ///   Implement your actual logging sink here.
    /// </param>
    /// <remarks>
    ///   Order dependency: This has to be done before initializing any trace sources 
    /// </remarks>
    public TraceBusListener(OnLogEventReceivedDelegate onLogEventReceived) {

      OnLogEventReceived = onLogEventReceived;

      Trace.Listeners.Add(this); // Self-register to .net runtime

      // Remark: This is a one-way-ticket. Removing (even disposing) a listener is futile, because all TraceSources
      // hold a reference. You would have to iterate all existing TraceSources first an remove the listener there.
      // This is impossible due to the fact that there's no (clean) way of getting all existing TraceSources.
      // See: https://stackoverflow.com/questions/10581448/add-remove-tracelistener-to-all-tracesources

    }

    public override void TraceEvent(
      TraceEventCache eventCache, string sourceName, TraceEventType eventType, int eventId, string formatString, params object[] args
    ) {

      if (OnLogEventReceived == null) {
        return;
      }

      // ...Map EventType => LogLevel...

      int level = 0; // Default: "Trace" (aka "Verbose")

      switch (eventType) {
        case TraceEventType.Critical: level = 5; break; // (aka "Fatal")
        case TraceEventType.Error: level = 4; break;
        case TraceEventType.Warning: level = 3; break;
        case TraceEventType.Information: level = 2; break;
        case TraceEventType.Transfer: level = 1; break; // There is no "Debug" EventType => (mis)use something else        
      }

      // Refine the System.Diagnostics TraceEvent to become a SmartStandards LogEvent...

      long sourceLineId = 0;
      string audienceToken = null;
      string messageTemplate = null;

      TokenizeFormatString(formatString, out sourceLineId, out audienceToken, out messageTemplate);

      // Fallback: If the audience token could not be parsed from the formatString, try a lookup

      if (audienceToken == "???") {
        TraceSourcesToAudienceMapping.TryGetValue(sourceName, out audienceToken);
      }

      // Pass the LogEvent to the callback method

      OnLogEventReceived.Invoke(audienceToken, level, sourceName, sourceLineId, eventId, messageTemplate, args);
    }

    public override void Write(string message) { } // Not needed

    public override void WriteLine(string message) { }// Not needed

    private void TokenizeFormatString(
      string formatString, out long sourceLineId, out string audienceToken, out string messageTemplate
    ) {

      sourceLineId = 0;
      audienceToken = "???";
      messageTemplate = formatString;

      // " SourceLineId [AudienceToken]: MessageTemplate"
      //  01234567890
      // " 0 [Ins]: MessageTemplate"

      if (formatString == null || formatString.Length < 9) return;

      if (formatString[0] != ' ') return;

      int rightOfSourceLineId = formatString.IndexOf(' ', 1);
      int leftOfAudience = formatString.IndexOf('[');
      int rightOfAudience = formatString.IndexOf("]:");

      if (
        rightOfSourceLineId > leftOfAudience ||
        leftOfAudience > rightOfAudience ||
        rightOfAudience - leftOfAudience != 4
      ) return;

      string sourceLineIdAsString = formatString.Substring(1, rightOfSourceLineId);

      if (!long.TryParse(sourceLineIdAsString, out sourceLineId)) return;

      audienceToken = formatString.Substring(leftOfAudience + 1, 3);

      int beginOfMessageTemplate = rightOfAudience + 3;

      if (formatString.Length >= beginOfMessageTemplate) {
        messageTemplate = formatString.Substring(beginOfMessageTemplate).Replace("{{", "{").Replace("}}", "}");
      } else {
        messageTemplate = "";
      }
    }

  }
}
