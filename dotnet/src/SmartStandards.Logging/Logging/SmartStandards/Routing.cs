using Logging.SmartStandards.Sinks;
using Logging.SmartStandards.Transport;
using System;

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
    ///   Emit textualized exceptions (as message) to the TraceBus (instead of the original exception as arg).
    /// </summary>
    public static bool TraceBusExceptionsTextualizedToggle { get; set; }

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

    public static void UseConsoleSink() {
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
      string audience, int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate, object[] args
    ) {
      CustomBusFeed.OnEmitMessage?.Invoke(audience, level, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    private static void PassTracedExceptionToCustomBus(
      string audience, int level, string sourceContext, long sourceLineId, int kindId, Exception ex
    ) {
      CustomBusFeed.OnEmitException?.Invoke(audience, level, sourceContext, sourceLineId, kindId, ex);
    }

  }
}
