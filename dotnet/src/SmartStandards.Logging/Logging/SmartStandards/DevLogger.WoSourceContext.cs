using System;
using System.Reflection;

namespace Logging.SmartStandards {

  public partial class DevLogger {

    private static string CurrentSourceContext {
      get {
        return Assembly.GetExecutingAssembly().GetName().Name;
      }
    }

    #region MessageTemplate only

    public static void LogTrace(string messageTemplate, params object[] args) {
      LogTrace(0, 0, messageTemplate, args);
    }

    public static void LogDebug(string messageTemplate, params object[] args) {
      LogDebug(0, 0, messageTemplate, args);
    }

    public static void LogInformation(string messageTemplate, params object[] args) {
      LogInformation(0, 0, messageTemplate, args);
    }

    public static void LogWarning(string messageTemplate, params object[] args) {
      LogWarning(0, 0, messageTemplate, args);
    }

    public static void LogError(string messageTemplate, params object[] args) {
      LogError(0, 0, messageTemplate, args);
    }

    public static void LogCritical(string messageTemplate, params object[] args) {
      LogCritical(0, 0, messageTemplate, args);
    }

    #endregion

    #region KindEnumElement only

    public static void LogTrace(Enum kindEnumElement, params object[] args) {
      LogTrace(0, kindEnumElement, args);
    }

    public static void LogDebug(Enum kindEnumElement, params object[] args) {
      LogDebug(0, kindEnumElement, args);
    }

    public static void LogInformation(Enum kindEnumElement, params object[] args) {
      LogInformation(0, kindEnumElement, args);
    }

    public static void LogWarning(Enum kindEnumElement, params object[] args) {
      LogWarning(0, kindEnumElement, args);
    }

    public static void LogError(Enum kindEnumElement, params object[] args) {
      LogError(0, kindEnumElement, args);
    }

    public static void LogCritical(Enum kindEnumElement, params object[] args) {
      LogCritical(0, kindEnumElement, args);
    }

    #endregion

    #region Exception only

    public static void LogTrace(Exception ex) {
      LogTrace(0, ex);
    }

    public static void LogDebug(Exception ex) {
      LogDebug(0, ex);
    }

    public static void LogInformation(Exception ex) {
      LogInformation(0, ex);
    }

    public static void LogWarning(Exception ex) {
      LogWarning(0, ex);
    }

    public static void LogError(Exception ex) {
      LogError(0, ex);
    }

    public static void LogCritical(Exception ex) {
      LogCritical(0, ex);
    }

    #endregion

    #region Ids and MessageTemplate

    public static void LogTrace(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(0, CurrentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogDebug(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(1, CurrentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogInformation(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(2, CurrentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogWarning(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(3, CurrentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogError(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(4, CurrentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void LogCritical(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      Log(5, CurrentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    #endregion

    #region Ids and KindEnumElement

    public static void LogTrace(long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(0, CurrentSourceContext, sourceLineId, kindEnumElement, args);
    }
    public static void LogDebug(long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(1, CurrentSourceContext, sourceLineId, kindEnumElement, args);
    }
    public static void LogInformation(long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(2, CurrentSourceContext, sourceLineId, kindEnumElement, args);
    }
    public static void LogWarning(long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(3, CurrentSourceContext, sourceLineId, kindEnumElement, args);
    }
    public static void LogError(long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(4, CurrentSourceContext, sourceLineId, kindEnumElement, args);
    }
    public static void LogCritical(long sourceLineId, Enum kindEnumElement, params object[] args) {
      Log(5, CurrentSourceContext, sourceLineId, kindEnumElement, args);
    }
    #endregion

    #region SourceLineId and Exception

    public static void LogTrace(long sourceLineId, Exception ex) {
      Log(0, CurrentSourceContext, sourceLineId, ex);
    }
    public static void LogDebug(long sourceLineId, Exception ex) {
      Log(1, CurrentSourceContext, sourceLineId, ex);
    }
    public static void LogInformation(long sourceLineId, Exception ex) {
      Log(2, CurrentSourceContext, sourceLineId, ex);
    }
    public static void LogWarning(long sourceLineId, Exception ex) {
      Log(3, CurrentSourceContext, sourceLineId, ex);
    }
    public static void LogError(long sourceLineId, Exception ex) {
      Log(4, CurrentSourceContext, sourceLineId, ex);
    }
    public static void LogCritical(long sourceLineId, Exception ex) {
      Log(5, CurrentSourceContext, sourceLineId, ex);
    }

    #endregion

  }
}
