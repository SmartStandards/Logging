using System;
using System.Linq;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Logs messages to the "Protocol" channel which is intended to be consumed by the (end-)user of a component.
  /// </summary>
  /// <remarks>
  ///   This is the only "public" channel that targets an audience that is not the provider of a component.
  ///   The messages should reflect issues that were caused at runtime by a user (or service request)
  ///   and can be fixed by him (or the developer of the requesting component).
  ///   Examples: Wrong input, insufficient permissions, etc.
  /// </remarks>
  public class ProtocolLogger : LoggerBase<ProtocolLogger> {

    /// <summary>
    ///   Channel name as used by the default log handler as trace source name.
    ///   Not used, if you override the log handler.
    /// </summary>
    public const string ChannelName = "Pro";

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

      LoggerBase<ProtocolLogger>.AwaitsInputFromTracing = forwardTracingInputToLogMehod;
      if (forwardTracingInputToLogMehod && !SmartStandardsTraceLogPipe.IsInitialized) {
        //ensure, that the pipe is up and running
        SmartStandardsTraceLogPipe.InitializeAsLoggerInput();
      }

      if (forwardDirectInputToTracing) {
        LoggerBase<ProtocolLogger>.InternalLogMethod = (
          (audience, viaTrc, level, id, msg, args) => {
            logMethod.Invoke(audience, level, id, msg, args);
            DefaultLogToTraceMethod(audience, viaTrc, level, id, msg, ConcatMirrorArgArray(args));
          }
        );
      }
      else {
        LoggerBase<ProtocolLogger>.InternalLogMethod = (
          (audience, viaTrc, level, id, messageTemplate, args) => logMethod.Invoke(audience, level, id, messageTemplate, args)
        );
      }

      if (logExceptionMethod != null) {
        if (forwardDirectInputToTracing) {
          LoggerBase<ProtocolLogger>.InternalExceptionLogMethod = (
            (audience, viaTrc, level, id, ex) => {
              logExceptionMethod.Invoke(audience, level, id, ex);
              DefaultLogToTraceMethod(audience, viaTrc, level, id, ex.Serialize(), MirrorArgArray);
            }
          );
        }
        else {
          LoggerBase<ProtocolLogger>.InternalExceptionLogMethod = (
            (audience, viaTrc, level, id, ex) => logExceptionMethod.Invoke(audience, level, id, ex)
          );
        }
      }
      else {
        LoggerBase<InfrastructureLogger>.InternalExceptionLogMethod = null;
      }

    }

    public static void ResetRedirection() {
      LoggerBase<ProtocolLogger>.AwaitsInputFromTracing = false;
      LoggerBase<ProtocolLogger>.InternalLogMethod = null;
      LoggerBase<ProtocolLogger>.InternalExceptionLogMethod = null;
    }

  }

}
