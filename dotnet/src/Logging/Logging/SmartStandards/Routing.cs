using Logging.SmartStandards.Transport;
using System;

namespace Logging.SmartStandards {

  public class Routing {

    private static bool _PreStagedTraceBusExceptionRenderingToggle;

    public static bool TraceBusExceptionRenderingToggle {
      get {
        if (_InternalTraceBusFeed != null) {
          return _InternalTraceBusFeed.ExceptionRenderingToggle;
        } else {
          return _PreStagedTraceBusExceptionRenderingToggle;
        }
      }
      set {
        _PreStagedTraceBusExceptionRenderingToggle = value;
        if (_InternalTraceBusFeed != null) {
          _InternalTraceBusFeed.ExceptionRenderingToggle = value;
        }
      }
    }

    private static TraceBusFeed _InternalTraceBusFeed;

    internal static TraceBusFeed InternalTraceBusFeed {
      get {
        if (_InternalTraceBusFeed == null) {
          _InternalTraceBusFeed = new TraceBusFeed();
          // ^ The new TraceBusFeed just connected to all currently existing trace listeners...
          if (_InternalTraceBusListener != null) {// ... if our own listener has been already established...
            _InternalTraceBusFeed.IgnoredListeners.Add(_InternalTraceBusListener); // ...we do not want to receive our own feed.
          }
          _InternalTraceBusFeed.ExceptionRenderingToggle = _PreStagedTraceBusExceptionRenderingToggle;
        }
        return _InternalTraceBusFeed;
      }
    }

    private static TraceBusListener _InternalTraceBusListener;

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
    public static bool PassThruTraceBusToCustomBus {
      get {
        return _TraceBusToCustomBus;
      }
      set {
        if (value) {
          if (_InternalTraceBusListener == null) {
            _InternalTraceBusListener = new TraceBusListener(PassTracedMessageToCustomBus, PassTracedExceptionToCustomBus);
            if (_InternalTraceBusFeed != null) {
              // Normally this shouldn't be necessary, because the _InternalTraceBusFeed has already been established
              // (and connected to all recent listeners). Our listener is coming too late to the party and will never be wired up to
              // the _InternalTraceBusFeed. Anyway, just to be sure, we put ourselves on the ignore list:
              _InternalTraceBusFeed.IgnoredListeners.Add(_InternalTraceBusListener);
            }
          } else {
            _InternalTraceBusListener.OnMessageReceived = PassTracedMessageToCustomBus;
          }
        } else {
          _InternalTraceBusListener.OnMessageReceived = null;
        }
      }
    }

    public static void EnableEmittingToTraceBus(bool enable) {
      DevLoggerToTraceBus = enable;
      InsLoggerToTraceBus = enable;
      BizLoggerToTraceBus = enable;
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

      EnableEmittingToTraceBus(false);

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
