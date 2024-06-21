﻿using System;

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

      if (forwardTracingInputToLogMehod) {
        LoggerBase<ProtocolLogger>.AwaitsInputFromTracing = true;
        //ensure, that the pipe is up and running
        if (!SmartStandardsTraceLogPipe.IsInitialized) {
          SmartStandardsTraceLogPipe.InitializeAsLoggerInput();
        }
      }

      if (forwardDirectInputToTracing) {
        LoggerBase<ProtocolLogger>.InternalLogMethod = (
          (src, viaTrc, lvl, id, msg, args) => {
            logMethod.Invoke(src, lvl, id, msg, args);
            DefaultLogToTraceMethod(src, viaTrc, lvl, id, msg, args);
          }
        );
      }
      else {
        LoggerBase<ProtocolLogger>.InternalLogMethod = (
          (src, viaTrc, lvl, id, msg, args) => logMethod.Invoke(src, lvl, id, msg, args)
        );
      }

      if (logExceptionMethod != null) {
        if (forwardDirectInputToTracing) {
          LoggerBase<ProtocolLogger>.InternalExceptionLogMethod = (
            (src, viaTrc, lvl, id, ex) => {
              logExceptionMethod.Invoke(src, lvl, id, ex);
              DefaultLogToTraceMethod(src, viaTrc, lvl, id, ex.Serialize(), null);
            }
          );
        }
        else {
          LoggerBase<ProtocolLogger>.InternalExceptionLogMethod = (
            (src, viaTrc, lvl, id, ex) => logExceptionMethod.Invoke(src, lvl, id, ex)
          );
        }
      }

    }

  }

}
