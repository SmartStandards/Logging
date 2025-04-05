using System;

namespace Logging.SmartStandards {

  public partial class DevLogger {

    public static void LogTrace(string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(0, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogTrace(string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(0, sourceContext, sourceLineId, kindEnumElement, args);
    }

    public static void LogTrace(string sourceContext, long sourceLineId, Exception ex) {
      Log(0, sourceContext, sourceLineId, ex);
    }

    public static void LogDebug(string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(1, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogDebug(string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(1, sourceContext, sourceLineId, kindEnumElement, args);
    }

    public static void LogDebug(string sourceContext, long sourceLineId, Exception ex) {
      Log(1, sourceContext, sourceLineId, ex);
    }

    public static void LogInformation(string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(2, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogInformation(string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(2, sourceContext, sourceLineId, kindEnumElement, args);
    }

    public static void LogInformation(string sourceContext, long sourceLineId, Exception ex) {
      Log(2, sourceContext, sourceLineId, ex);
    }

    public static void LogWarning(string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(3, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogWarning(string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(3, sourceContext, sourceLineId, kindEnumElement, args);
    }

    public static void LogWarning(string sourceContext, long sourceLineId, Exception ex) {
      Log(3, sourceContext, sourceLineId, ex);
    }

    public static void LogError(string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(4, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogError(string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(4, sourceContext, sourceLineId, kindEnumElement, args);
    }

    public static void LogError(string sourceContext, long sourceLineId, Exception ex) {
      Log(4, sourceContext, sourceLineId, ex);
    }

    public static void LogCritical(string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(5, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogCritical(string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(5, sourceContext, sourceLineId, kindEnumElement, args);
    }

    public static void LogCritical(string sourceContext, long sourceLineId, Exception ex) {
      Log(5, sourceContext, sourceLineId, ex);
    }
  }
}
