using System;
using System.Linq;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Logs messages to the "Infrastructure" channel which is intended to be consumed by the hoster of a component.
  /// </summary>
  /// <remarks>
  ///   The character of the messages shoud be "for internal use".
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
    ///   Hook. Inject your log emitting handler here.
    ///   Signature: void LogMethod(int level, int id, string messageTemplate, object[] args)
    ///   This will override the default handler (which uses System.Diagnostics.Trace to emit messages).
    /// </summary>
    public static void ConfigureRedirection(
      LogMethod logMethod,
      LogExceptionMethod logExceptionMethod = null,
      bool forwardDirectInputToTracing = false,
      bool forwardTracingInputToLogMehod = true
    ) {


      if (logMethod == null) {
        throw new ArgumentException("The logMethod must no be null! Please provide a dummy-lambda to disable the default behaviour (log to trace) / use 'ResetRedirection' to re-enable the default behaviour.");
      }

      LoggerBase<InfrastructureLogger>.AwaitsInputFromTracing = forwardTracingInputToLogMehod;
      if (forwardTracingInputToLogMehod && !SmartStandardsTraceLogPipe.IsInitialized) {
        //ensure, that the pipe is up and running
        SmartStandardsTraceLogPipe.InitializeAsLoggerInput();
      }

      if (forwardDirectInputToTracing) {
        LoggerBase<InfrastructureLogger>.InternalLogMethod = (
          ( src,  viaTrc, lvl, id,  msg, args) => {
            logMethod.Invoke(src, lvl, id, msg, args);
            DefaultLogToTraceMethod(src, viaTrc, lvl, id, msg, ConcatMirrorArgArray(args));
          }
        );
      }
      else {
        LoggerBase<InfrastructureLogger>.InternalLogMethod = (
          (src, viaTrc, lvl, id, msg, args) => logMethod.Invoke(src, lvl, id, msg, args)
        );
      }

      if (logExceptionMethod != null) {
        if (forwardDirectInputToTracing) {
          LoggerBase<InfrastructureLogger>.InternalExceptionLogMethod = (
            (src, viaTrc, lvl, id, ex) => {
              logExceptionMethod.Invoke(src, lvl, id, ex);
              DefaultLogToTraceMethod(src, viaTrc, lvl, id, ex.Serialize(), MirrorArgArray);
            }
          );
        }
        else {
          LoggerBase<InfrastructureLogger>.InternalExceptionLogMethod = (
            (src, viaTrc, lvl, id, ex) => logExceptionMethod.Invoke(src, lvl, id, ex)
          );
        }
      }
      else {
        LoggerBase<InfrastructureLogger>.InternalExceptionLogMethod = null;
      }

    }

    public static void ResetRedirection() {
      LoggerBase<InfrastructureLogger>.AwaitsInputFromTracing = false;
      LoggerBase<InfrastructureLogger>.InternalLogMethod = null;
      LoggerBase<InfrastructureLogger>.InternalExceptionLogMethod = null;
    }

  }

}
