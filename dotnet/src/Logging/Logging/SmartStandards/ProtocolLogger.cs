using System;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Logs messages to the "Protocol" channel which is intended to be consumed by the (end-)user of a component.
  /// </summary>
  /// <remarks>
  ///   This is the only "public" channel that targets an audience that is not the provider of a component.
  ///   Usually, these messages are presented at the UI (or returned as service response).
  ///   The messages should reflect issues that were caused at runtime by a user (or service request) 
  ///   and can be fixed by him (or the developer of the requesting component).
  ///   Examples: Wrong input, bad configuration, insufficient permissions, etc.
  /// </remarks>
  public class ProtocolLogger : LoggerBase<ProtocolLogger> {

    /// <summary>
    ///   Channel name as used by the default log handler as trace source name.
    ///   Not used, if you override the log handler.
    /// </summary>
    public const string ChannelName = "Pro";

    /// <summary>
    ///   Hook. Inject your log handler delegate here.
    ///   This will override the default log handler (which uses System.Diagnostics.Trace to emit messages).
    /// </summary>
    protected static Action<int, int, string, object[]> LogMethod {
      get {
        return LoggerBase<ProtocolLogger>.InternalLogMethod;
      }
      set {
        LoggerBase<ProtocolLogger>.InternalLogMethod = value;
      }
    }

  }

}
