using System;
using Logging.SmartStandards.Internal;
using Logging.SmartStandards.Transport;

namespace Logging.SmartStandards {

  public class Routing {

    private static bool _BizLoggerToTraceBus = true;

    private static bool _DevLoggerToTraceBus = true;

    private static bool _InsLoggerToTraceBus = true;

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

          DebuggingGraceTimer.Start();
        }
        return _InternalTraceBusFeed;
      }
    }


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
        ShutDownTraceBusFeedIfAppropriate();
      }
    }

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
        ShutDownTraceBusFeedIfAppropriate();
      }
    }


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
        ShutDownTraceBusFeedIfAppropriate();
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
    ///   Convenience method to set all routings to trace bus at once.
    /// </summary>
    /// <param name="enable"> Set True to enable emitting to trace bus. </param>
    /// <remarks>
    ///   Calling this method (with true) will always recreate the internal trace source and wire up to all currently 
    ///   registered trace listeners.
    /// </remarks>
    public static void ReEnableEmittingToTraceBus(bool enable) {

      _InternalTraceBusFeed = null;
      // ^ we want to force a new instance because it should connect to TraceListeners that may have been registered meanwhile

      DevLoggerToTraceBus = enable;
      InsLoggerToTraceBus = enable;
      BizLoggerToTraceBus = enable;
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

    private static void ShutDownTraceBusFeedIfAppropriate() {

      if (_InternalTraceBusFeed == null) return;

      if (!_BizLoggerToTraceBus && !_DevLoggerToTraceBus && !_InsLoggerToTraceBus) {
        _InternalTraceBusFeed = null;
      }
    }
  }
}
