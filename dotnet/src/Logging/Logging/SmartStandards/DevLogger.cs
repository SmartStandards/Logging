using System;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Logs messages to the "Developer" channel which is intended to be consumed by the developer of a component.
  /// </summary>
  /// <remarks>
  ///   The character of the messages shoud be "for internal use".
  ///   The messages should reflect issues that were caused at design time by a bug in the code and can be fixed by developers.
  ///   Examples: Invalid argument, out of range error, etc.
  /// </remarks>
  public class DevLogger : LoggerBase<DevLogger> {

    /// <summary>
    ///   Channel name as used by the default log handler as trace source name.
    ///   Not used, if you override the log handler.
    /// </summary>
    public const string ChannelName = "Dev";

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

      LoggerBase<DevLogger>.AwaitsInputFromTracing = forwardTracingInputToLogMehod;
      if (forwardTracingInputToLogMehod && !SmartStandardsTraceLogPipe.IsInitialized) {
        //ensure, that the pipe is up and running
        SmartStandardsTraceLogPipe.InitializeAsLoggerInput();
      }

      if (forwardDirectInputToTracing) {
        LoggerBase<DevLogger>.InternalLogMethod = (
          (audience, viaTrc, level, id, messageTemplate, args) => {
            logMethod.Invoke(audience, level, id, messageTemplate, args);
            DefaultLogToTraceMethod(audience, viaTrc, level, id, messageTemplate, ConcatMirrorArgArray(args));
          }
        );
      } else {
        LoggerBase<DevLogger>.InternalLogMethod = (
          (audience, viaTrc, level, id, messageTemplate, args) => logMethod.Invoke(audience, level, id, messageTemplate, args)
        );
      }

      if (logExceptionMethod != null) {
        if (forwardDirectInputToTracing) {
          LoggerBase<DevLogger>.InternalExceptionLogMethod = (
            (audience, viaTrc, level, id, ex) => {
              logExceptionMethod.Invoke(audience, level, id, ex);
              DefaultLogToTraceMethod(audience, viaTrc, level, id, ex.Serialize(), MirrorArgArray);
            }
          );
        } else {
          LoggerBase<DevLogger>.InternalExceptionLogMethod = (
            (audience, viaTrc, level, id, ex) => logExceptionMethod.Invoke(audience, level, id, ex)
          );
        }
      } else {
        LoggerBase<DevLogger>.InternalExceptionLogMethod = null;
      }

    }

    public static void ResetRedirection() {
      LoggerBase<DevLogger>.AwaitsInputFromTracing = false;
      LoggerBase<DevLogger>.InternalLogMethod = null;
      LoggerBase<DevLogger>.InternalExceptionLogMethod = null;
    }

  }

}
