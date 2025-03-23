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
    /// <remarks>
    ///   This must be done as early as possible: Before any TraceSources are instantiated, because they only wire up
    ///   to already existing TraceListeners.
    /// </remarks>
    public static bool EnablePassThruTraceBusToCustomBus {
      get {
        return _TraceBusToCustomBus;
      }
      set {
        if (value) {
          if (!TraceBusListener.IsInitialized) {
            TraceBusListener smartStandardsTraceBusListener = TraceBusListener.Initialize(PassTraceEventToCustomBus);
            // We do not want to receive our own feed
            TraceBusFeed.IgnoredListeners.Add(smartStandardsTraceBusListener);
          } else {
            TraceBusListener.OnLogEventReceived = PassTraceEventToCustomBus;
          }
        } else {
          TraceBusListener.OnLogEventReceived = null;
        }
      }
    }
    
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
      
      DevLoggerToTraceBus = false;
      InsLoggerToTraceBus = false;
      BizLoggerToTraceBus = false;

      DevLoggerToCustomBus = true;
      InsLoggerToCustomBus = true;
      BizLoggerToCustomBus = true;
    }        

    private static void PassTraceEventToCustomBus(string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, object[] args) {

      if ((args != null) && (args.Length > 0) && (args[0] is Exception)) {
        Exception ex = (Exception)args[0];
        CustomBusFeed.OnEmitException?.Invoke(audience, level, sourceContext, sourceLineId, ex);
      } else {
        CustomBusFeed.OnEmitMessage?.Invoke(audience, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
      }

    }
    
  }
}
