using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Logging.SmartStandards.Textualization;

[assembly: InternalsVisibleTo("Logging.Tests")]

namespace Logging.SmartStandards.Transport {

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

    public delegate bool FilterIncomingTraceEventDelegate(int eventType, string sourceName, string formatString);

    public delegate void OnMessageReceivedDelegate(string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, object[] args);

    public delegate void OnExceptionReceivedDelegate(string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex);

    public bool IsActive { get; set; } = true;

    public OnMessageReceivedDelegate OnMessageReceived { get; set; }

    public OnExceptionReceivedDelegate OnExceptionReceived { get; set; }

    /// <summary>
    ///   3rd party trace sources may not provide an audience. 
    ///   Therefore you can map their source name to a target audience.
    /// </summary>
    public Dictionary<string, string> TraceSourcesToAudienceMapping { get; set; } = new Dictionary<string, string>();

    /// <summary>
    ///   Constructor.
    /// </summary>
    protected TraceBusListener() {
      // Constructor is private, because this must be a single instance
    }

    /// <summary>
    ///   Constructor. Builds up a trace listener and registers it immediately to Trace.Listeners.
    /// </summary>
    /// <param name="onMessageReceived">
    ///   Callback delegate which is called for each recieved log entry. 
    ///   Signature: onLog(string channelName, int level, int id, string messageTemplate, object[] args)
    ///   Implement your actual logging sink here.
    /// </param>
    /// <remarks>
    ///   Order dependency: This has to be done before initializing any trace sources.
    /// </remarks>
    public TraceBusListener(OnMessageReceivedDelegate onMessageReceived, OnExceptionReceivedDelegate onExceptionReceived) {

      this.OnMessageReceived = onMessageReceived;

      this.OnExceptionReceived = onExceptionReceived;

      Trace.Listeners.Add(this); // Self-register to .net runtime

      // Remark: This is a one-way-ticket. Removing (even disposing) a listener is futile, because all TraceSources
      // hold a reference. You would have to iterate all existing TraceSources first an remove the listener there.
      // This is impossible due to the fact that there's no (clean) way of getting all existing TraceSources.
      // See: https://stackoverflow.com/questions/10581448/add-remove-tracelistener-to-all-tracesources

    }

    public override void TraceEvent(
      TraceEventCache eventCache, string sourceName, TraceEventType eventType, int eventKindId, string formatString, params object[] args
    ) {

      if (!this.IsActive || this.OnMessageReceived == null) {
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

      LogParaphParser.TokenizeMetaDataRightPart(formatString, out sourceLineId, out audienceToken, out messageTemplate);

      messageTemplate = messageTemplate.Replace("{{", "{").Replace("}}", "}");

      // Fallback: If the audience token could not be parsed from the formatString, try a lookup

      if (audienceToken == "") {
        if (!this.TraceSourcesToAudienceMapping.TryGetValue(sourceName, out audienceToken)) audienceToken = "Dev";
      }

      // Pass the LogEvent to the callback method

      if ((args != null) && (args.Length > 0) && (args[0] is Exception)) {
        Exception ex = (Exception)args[0];
        this.OnExceptionReceived.Invoke(audienceToken, level, sourceName, sourceLineId, eventKindId, ex);
      } else {
        this.OnMessageReceived.Invoke(audienceToken, level, sourceName, sourceLineId, eventKindId, messageTemplate, args);
      }
    }

    public override void Write(string message) { } // Not needed

    public override void WriteLine(string message) { }// Not needed

  }
}
