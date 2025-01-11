using System;

namespace Logging.SmartStandards {

  public partial class BizLogger {

    public static void LogTrace(string sourceContext, int id, string messageTemplate, params object[] args) {
      Log(sourceContext, 0, id, messageTemplate, args);
    }

    public static void LogTrace(string sourceContext, Enum logTemplate, params object[] args) {
      Log(sourceContext, 0, logTemplate, args);
    }

    public static void LogTrace(string sourceContext, Exception ex) {
      Log(sourceContext, 0, ex);
    }

    public static void LogDebug(string sourceContext, int id, string messageTemplate, params object[] args) {
      Log(sourceContext, 1, id, messageTemplate, args);
    }

    public static void LogDebug(string sourceContext, Enum logTemplate, params object[] args) {
      Log(sourceContext, 1, logTemplate, args);
    }

    public static void LogDebug(string sourceContext, Exception ex) {
      Log(sourceContext, 1, ex);
    }

    public static void LogInformation(string sourceContext, int id, string messageTemplate, params object[] args) {
      Log(sourceContext, 2, id, messageTemplate, args);
    }

    public static void LogInformation(string sourceContext, Enum logTemplate, params object[] args) {
      Log(sourceContext, 2, logTemplate, args);
    }

    public static void LogInformation(string sourceContext, Exception ex) {
      Log(sourceContext, 2, ex);
    }

    public static void LogWarning(string sourceContext, int id, string messageTemplate, params object[] args) {
      Log(sourceContext, 3, id, messageTemplate, args);
    }

    public static void LogWarning(string sourceContext, Enum logTemplate, params object[] args) {
      Log(sourceContext, 3, logTemplate, args);
    }

    public static void LogWarning(string sourceContext, Exception ex) {
      Log(sourceContext, 3, ex);
    }

    public static void LogError(string sourceContext, int id, string messageTemplate, params object[] args) {
      Log(sourceContext, 4, id, messageTemplate, args);
    }

    public static void LogError(string sourceContext, Enum logTemplate, params object[] args) {
      Log(sourceContext, 4, logTemplate, args);
    }

    public static void LogError(string sourceContext, Exception ex) {
      Log(sourceContext, 4, ex);
    }

    public static void LogCritical(string sourceContext, int id, string messageTemplate, params object[] args) {
      Log(sourceContext, 5, id, messageTemplate, args);
    }

    public static void LogCritical(string sourceContext, Enum logTemplate, params object[] args) {
      Log(sourceContext, 5, logTemplate, args);
    }

    public static void LogCritical(string sourceContext, Exception ex) {
      Log(sourceContext, 5, ex);
    }

    public static void LogReturnCodeAsError(string sourceContext, int id, string messageTemplate, object[] args) {

      if (id < 0) {
        LogError(sourceContext, -id, messageTemplate, args);
      } else {
        LogInformation(sourceContext, id, messageTemplate, args);
      }
    }

    public static void LogReturnCodeAsWarning(string sourceContext, int id, string messageTemplate, object[] args) {

      if (id < 0) {
        LogWarning(sourceContext, -id, messageTemplate, args);
      } else {
        LogInformation(sourceContext, id, messageTemplate, args);
      }
    }

  }
}
