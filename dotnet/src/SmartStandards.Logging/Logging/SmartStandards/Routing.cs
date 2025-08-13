using Logging.SmartStandards.Sinks;
using Logging.SmartStandards.Transport;
using System;
using System.Collections.Generic;

namespace Logging.SmartStandards {

  public class Routing {

    private static TraceBusFeed _InternalTraceBusFeed;

    private static TraceBusListener _InternalTraceBusListener;

    /// <summary> Static constructor </summary>
    static Routing() {

      // TraceListeners must be registered as early as possible so they can be found by TraceSources.

      _InternalTraceBusListener = new TraceBusListener(PassTracedMessageToCustomBus, PassTracedExceptionToCustomBus) {
        Name = "SmartStandards395316649"
      };

    }

    /// <summary>
    ///   Determines how the TraceBus should pass LogEvents to each listener 
    ///   - Ready-to-read: WriteLine() is called with MessageTemplate's placeholders resolved and exceptions textualized.
    ///   - Raw: TraceEvent() is called with MessageTemplate and Args unresolved. Exceptions are passed as first arg.
    /// </summary>
    /// <remarks>
    ///   Ready-to-read mode will call the Listeners' WriteLine() method instead of TraceEvent().
    ///   This wil bypass WriteHeader(), WriteFooter() and anything depending on the TraceEvenType, because
    ///   there is no TraceEvenType parameter in the WriteLine() method.
    /// </remarks>
    /// <returns>
    ///   A list containing the names of the listeners to be handled in raw mode.
    /// </returns>
    public static List<string> TraceBusRawMode {
      get {
        return InternalTraceBusFeed.RawModeListeners;
      }
    }

    internal static TraceBusFeed InternalTraceBusFeed {
      get {
        if (_InternalTraceBusFeed == null) {

          _InternalTraceBusFeed = new TraceBusFeed();
          // ^ The new TraceBusFeed just connected to all currently existing trace listeners...

          _InternalTraceBusFeed.IgnoredListeners.Add(_InternalTraceBusListener.Name); // ...we do not want to receive our own feed.
        }
        return _InternalTraceBusFeed;
      }
    }

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


    /// <summary>
    ///   Enable/disable passing through events from System.Diagnostics.Trace to CustomBusFeed.
    /// </summary>
    public static bool TraceBusToCustomBus {
      get {
        return _InternalTraceBusListener.IsActive;
      }
      set {
        _InternalTraceBusListener.IsActive = value;
      }
    }

    /// <summary>
    ///   Convenience method to wire up the built-in console sink to the custom bus.
    /// </summary>
    /// <param name="level">
    ///   The minimum level to be written by the console sink (default is 0).
    ///   0 = Trace, 1 = Debug, 2 = Information, 3 = Warning, 4 = Error, 5 = Critical
    /// </param>
    public static void UseConsoleSink(int level = 0) {
      ConsoleSink.Level = level;
      Routing.UseCustomBus(ConsoleSink.WriteMessage, ConsoleSink.WriteException);
    }

    /// <summary>
    ///   Convenience method to change the routing target from System.Diagnostics.Trace to any custom target.
    /// </summary>
    /// <param name="onEmitMessage"> 
    ///   Custom delegate, that will be called by any logger for any emit (except exceptions).
    ///   Put your code here to forward any log message to any forther target(s).
    /// </param>
    /// <param name="onEmitException"> 
    ///   Custom delegate, that will be called by any logger for any emit exception.
    ///   Put your code here to forward any exception to any forther target(s).
    /// </param>
    /// <remarks>
    ///   This will do the following:
    ///     Register your delegate to CustomBusFeed.
    ///     Create a delegate for logging exceptions (based on your delegate) and register it to CustomBusFeed.
    ///     Enable logging to the CustomBusFeed (by setting the appropriate Routing properties).
    ///     Disable logging to the TraceBusFeed (by setting the appropriate Routing properties).
    /// </remarks>
    public static void UseCustomBus(CustomBusFeed.EmitMessageDelegate onEmitMessage, CustomBusFeed.EmitExceptionDelegate onEmitException) {

      CustomBusFeed.OnEmitMessage = onEmitMessage;
      CustomBusFeed.OnEmitException = onEmitException;

      DevLoggerToCustomBus = true;
      InsLoggerToCustomBus = true;
      BizLoggerToCustomBus = true;
    }

    private static void PassTracedMessageToCustomBus(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, object[] args
    ) {
      CustomBusFeed.OnEmitMessage?.Invoke(audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
    }

    private static void PassTracedExceptionToCustomBus(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex
    ) {
      CustomBusFeed.OnEmitException?.Invoke(audience, level, sourceContext, sourceLineId, eventKindId, ex);
    }

  }
}
