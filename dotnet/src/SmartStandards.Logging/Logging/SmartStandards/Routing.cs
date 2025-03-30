using System;
using Logging.SmartStandards.Internal;
using Logging.SmartStandards.Transport;

namespace Logging.SmartStandards {

  public class Routing {

    private static TraceBusListener _InternalTraceBusListener = (
      new TraceBusListener(PassTracedMessageToCustomBus, PassTracedExceptionToCustomBus)
    // TraceListeners must be registered as early as possible so they can be found by TraceSources.
    );

    private static bool _TraceBusExceptionsTextualizedTogglePreStaged = true;

    /// <summary>
    ///   Emit textualized exceptions (as message) to the TraceBus (instead of the original exception as arg).
    /// </summary>
    public static bool TraceBusExceptionsTextualizedToggle {
      get {
        if (_InternalTraceBusFeed != null) {
          return _InternalTraceBusFeed.ExceptionsTextualizedToggle;
        } else {
          return _TraceBusExceptionsTextualizedTogglePreStaged;
        }
      }
      set {
        _TraceBusExceptionsTextualizedTogglePreStaged = value;
        if (_InternalTraceBusFeed != null) {
          _InternalTraceBusFeed.ExceptionsTextualizedToggle = value;
        }
      }
    }

    private static TraceBusFeed _InternalTraceBusFeed;

    internal static TraceBusFeed InternalTraceBusFeed {
      get {
        if (_InternalTraceBusFeed == null) {

          _InternalTraceBusFeed = new TraceBusFeed();
          // ^ The new TraceBusFeed just connected to all currently existing trace listeners...

          _InternalTraceBusFeed.IgnoredListeners.Add(_InternalTraceBusListener); // ...we do not want to receive our own feed.

          _InternalTraceBusFeed.ExceptionsTextualizedToggle = _TraceBusExceptionsTextualizedTogglePreStaged;

          DebuggingGraceTimer.Start();
        }
        return _InternalTraceBusFeed;
      }
    }

    private static bool _DevLoggerToTraceBus = true;

    /// <summary>
    ///   Enable/disable logging to .NET System.Diagnostics.Trace (via TraceBusFeed)
    /// </summary>
    public static bool DevLoggerToTraceBus {
      get {
        return _DevLoggerToTraceBus;
      }
      set {
        _DevLoggerToTraceBus = value;
        DebuggingGraceTimer.IsCancelled = true;
      }
    }

    private static bool _InsLoggerToTraceBus = true;

    /// <summary>
    ///   Enable/disable logging to .NET System.Diagnostics.Trace (via TraceBusFeed)
    /// </summary>
    public static bool InsLoggerToTraceBus {
      get {
        return _InsLoggerToTraceBus;
      }
      set {
        _InsLoggerToTraceBus = value;
        DebuggingGraceTimer.IsCancelled = true;
      }
    }

    private static bool _BizLoggerToTraceBus = true;

    /// <summary>
    ///   Enable/disable logging to .NET System.Diagnostics.Trace (via TraceBusFeed)
    /// </summary>
    public static bool BizLoggerToTraceBus {
      get {
        return _BizLoggerToTraceBus;
      }
      set {
        _BizLoggerToTraceBus = value;
        DebuggingGraceTimer.IsCancelled = true;
      }
    }

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
        return _InternalTraceBusListener.IsActive;
      }
      set {
        _InternalTraceBusListener.IsActive = value;
      }
    }

    public static void EnableEmittingToTraceBus(bool enable) {
      // todo: Aufruf mit mehrmals true soll neue TraceSource-Instanz erzwingen

      DevLoggerToTraceBus = enable;
      InsLoggerToTraceBus = enable;
      BizLoggerToTraceBus = enable;
      // todo: wenn alle aus sind => TraceSource disposen, damit Reconnect (für neue Listener) möglich wird.
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
    public static void UseCustomBus(CustomBusFeed.EmitMessageDelegate onEmitMessage, CustomBusFeed.EmitExceptionDelegate onEmitException) {

      CustomBusFeed.OnEmitMessage = onEmitMessage;
      CustomBusFeed.OnEmitException = onEmitException;

      DevLoggerToCustomBus = true;
      InsLoggerToCustomBus = true;
      BizLoggerToCustomBus = true;
    }

    private static void PassTracedMessageToCustomBus(
      string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, object[] args
    ) {
      CustomBusFeed.OnEmitMessage?.Invoke(audience, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
    }

    private static void PassTracedExceptionToCustomBus(
      string audience, int level, string sourceContext, long sourceLineId, int eventId, Exception ex
    ) {
      CustomBusFeed.OnEmitException?.Invoke(audience, level, sourceContext, sourceLineId, eventId, ex);
    }

  }
}
