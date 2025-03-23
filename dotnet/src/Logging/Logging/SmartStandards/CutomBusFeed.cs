using System;

namespace Logging.SmartStandards {

  /// <summary>
  ///   (Static) Customizing hook for routing messages to any further target.
  /// </summary>
  public class CustomBusFeed {

    public delegate void EmitMessageDelegate(string sourceContext, string audience, int level, int id, string messageTemplate, object[] args);

    public delegate void EmitExceptionDelegate(string sourceContext, string audience, int level, Exception ex);

    /// <summary>
    ///   Customizing hook. Will be called by any SmartStandards logger (if enabled).
    ///   Register your routing delegate here to forward a log message to any further target.
    /// </summary>
    /// <remarks>
    ///   It is suggested to use the convenience method Routing.UseCustomBus() instead of doing a manual wire up.    ///   
    /// </remarks>
    public static EmitMessageDelegate OnEmitMessage { get; set; }

    /// <summary>
    ///   Customizing hook. Will be called by any SmartStandards logger.
    ///   Register your routing delegate here to forward an exception log message to any further target.
    /// </summary>
    /// <remarks>
    ///   It is suggested to use the convenience method Routing.UseCustomBus() instead of doing a manual wire up.
    /// </remarks>
    public static EmitExceptionDelegate OnEmitException { get; set; }

  }

}
