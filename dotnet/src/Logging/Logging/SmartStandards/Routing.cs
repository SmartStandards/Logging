using System.Collections.Generic;
using Logging.SmartStandards;
using static Logging.SmartStandards.CustomBusFeed;

namespace System.Logging.SmartStandards {

  public class Routing {

    /// <summary>
    ///   Enable/disable logging to .NET System.Diagnostics.Trace (via TraceBusFeed)
    /// </summary>
    public static bool DevLoggerToTraceBus { get; set; } = true;

    /// <summary>
    ///   Enable/disable logging to .NET System.Diagnostics.Trace (via TraceBusFeed)
    /// </summary>
    public static bool InsLoggerToTraceBus { get; set; } = true;

    /// <summary>
    ///   Enable/disable logging to .NET System.Diagnostics.Trace (via TraceBusFeed)
    /// </summary>
    public static bool BizLoggerToTraceBus { get; set; } = true;

    /// <summary>
    ///   Enable/disable logging to CustomBusFeed
    /// </summary>
    public static bool DevLoggerToCustomBus { get; set; }

    /// <summary>
    ///   Enable/disable logging to CustomBusFeed
    /// </summary>
    public static bool InsLoggerToCustomBus { get; set; }

    /// <summary>
    ///   Enable/disable logging to CustomBusFeed
    /// </summary>
    public static bool BizLoggerToCustomBus { get; set; }

    private static bool _TraceBusToCustomBus;

    /// <summary>
    ///   Enable/disable passing through events from System.Diagnostics.Trace to CustomBusFeed.
    /// </summary>
    public static bool TraceBusToCustomBus {
      get {
        return _TraceBusToCustomBus;
      }
      set {
        if (value) {
          if (!TraceBusListener.IsInitialized) {
            TraceBusListener.Initialize(PassTraceEventToCustomBus);
            TraceBusListener.OnFilterIncomingTraceEvent = FilterIncomingTraceEvent;
          } else {
            TraceBusListener.OnLogEventReceived = PassTraceEventToCustomBus;
          }
        } else {
          TraceBusListener.OnLogEventReceived = null;
        }
      }
    }

    public static Dictionary<string, string> TraceSourcesToAudienceMapping { get; set; } = new Dictionary<string, string>();

    /// <summary>
    ///   Convenience method to change the routing target from System.Diagnostics.Trace to any custom target.
    /// </summary>
    /// <param name="onEmitMessage"> 
    ///   Custom delegate, that will be called by any logger for any emit.
    ///   Put your code here to forward any log message to any forther target(s).
    /// </param>
    /// <remarks>
    ///   This will do the following:
    ///     Register your delegate to CustomBusFeed.
    ///     Create a delegate for logging exceptions (based on your delegate) and register it to CustomBusFeed.
    ///     Enable logging to the CustomBusFeed (by setting the appropriate Routing properties).
    ///     Disable logging to the TraceBusFeed (by setting the appropriate Routing properties).
    /// </remarks>
    public static void UseCustomBus(EmitMessageDelegate onEmitMessage) {

      CustomBusFeed.OnEmitMessage = onEmitMessage;

      CustomBusFeed.OnEmitException = (string sourceContext, string audience, int level, Exception ex) => {
        int id = ExceptionSerializer.GetGenericIdFromException(ex);
        string serializedException = ex.Serialize();
        CustomBusFeed.OnEmitMessage.Invoke(sourceContext, audience, level, id, serializedException, new object[] { ex });
      };

      DevLoggerToTraceBus = false;
      InsLoggerToTraceBus = false;
      BizLoggerToTraceBus = false;

      DevLoggerToCustomBus = true;
      InsLoggerToCustomBus = true;
      BizLoggerToCustomBus = true;
    }

    public static void UseCustomBus(EmitMessageDelegate onEmitMessage, EmitExceptionDelegate onEmitException) {

      CustomBusFeed.OnEmitMessage = onEmitMessage;

      CustomBusFeed.OnEmitException = onEmitException;

      DevLoggerToTraceBus = false;
      InsLoggerToTraceBus = false;
      BizLoggerToTraceBus = false;

      DevLoggerToCustomBus = true;
      InsLoggerToCustomBus = true;
      BizLoggerToCustomBus = true;

    }

    private static void PassTraceEventToCustomBus(string sourceContext, string audience, int level, int id, string messageTemplate, object[] args) {

      if ((args != null) && (args.Length > 0) && (args[0] is Exception)) {
        Exception ex = (Exception)args[0];
        CustomBusFeed.OnEmitException?.Invoke(sourceContext, audience, level, ex);
      } else {
        CustomBusFeed.OnEmitMessage?.Invoke(sourceContext, audience, level, id, messageTemplate, args);
      }

    }

    private static bool FilterIncomingTraceEvent(string sourceName, string formatString, out string audienceToken) {

      audienceToken = null;

      if (formatString != null && formatString.Length >= 6 && formatString[0] == '(' && formatString[4] == ')' && formatString[5] == ' ') {
        audienceToken = formatString.Substring(1, 3);
      }

      if (audienceToken == null) {
        TraceSourcesToAudienceMapping.TryGetValue(sourceName, out audienceToken);
      }

      if (audienceToken != "Dev" && audienceToken != "Ins" && audienceToken != "Biz") {
        // audienceTokens emitted by SmartStandards loggers are UPPERCASE => they'll be filtered out, because they must never be
        // passed thru from TraceBus to CustomBus, because they were directly emitted to CustomBus
        return false;
      }

      return (audienceToken != null);
    }

    private static bool SuppressIncomingTraceEvent(string sourceName, string formatString, out string audienceToken) {
      audienceToken = null;
      return false;
    }

  }
}
