using System;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Logs messages to the "Infrastructure" channel which is intended to be consumed by the hoster of a component.
  /// </summary>
  /// <remarks>
  ///   The character of the messages shoud be "for internal use".
  ///   Usually, these messages are stored in a file or database.
  ///   The messages should reflect issues that were caused at runtime by the environment and can be fixed by dev-ops people.
  ///   Examples: Low disk space, network timeouts, etc.
  /// </remarks>
  public class InfrastructureLogger : LoggerBase<InfrastructureLogger> {

    /// <summary>
    ///   Channel name as used by the default log handler as trace source name.
    ///   Not used, if you override the log handler.
    /// </summary>
    public const string ChannelName = "Ins";

    /// <summary>
    ///   Hook. Inject your log handler delegate here.
    ///   This will override the default log handler (which uses System.Diagnostics.Trace to emit messages).
    /// </summary>
    protected static Action<int, int, string, object[]> LogMethod {
      get {
        return LoggerBase<InfrastructureLogger>.InternalLogMethod;
      }
      set {
        LoggerBase<InfrastructureLogger>.InternalLogMethod = value;
      }
    }

  }

}
