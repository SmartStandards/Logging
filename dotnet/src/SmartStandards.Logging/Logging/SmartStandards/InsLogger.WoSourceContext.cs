using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Logging.SmartStandards {

  public partial class InsLogger {

    // NOTE: [MethodImpl(MethodImplOptions.NoInlining)]
    // is used to avoid wrong results from Assembly.GetCallingAssembly()

    #region MessageTemplate only

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogTrace(string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogTrace(currentSourceContext, 0, 0, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogDebug(string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogDebug(currentSourceContext, 0, 0, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogInformation(string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogInformation(currentSourceContext, 0, 0, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogWarning(string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogWarning(currentSourceContext, 0, 0, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogError(string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogError(currentSourceContext, 0, 0, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogCritical(string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogCritical(currentSourceContext, 0, 0, messageTemplate, args);
    }

    #endregion

    #region templateEnumElement only

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogTrace(Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogTrace(currentSourceContext, 0, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogDebug(Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogDebug(currentSourceContext, 0, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogInformation(Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogInformation(currentSourceContext, 0, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogWarning(Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogWarning(currentSourceContext, 0, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogError(Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogError(currentSourceContext, 0, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogCritical(Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogCritical(currentSourceContext, 0, templateEnumElement, args);
    }

    #endregion

    #region Exception only

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogTrace(Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogTrace(currentSourceContext, 0, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogDebug(Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogDebug(currentSourceContext, 0, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogInformation(Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogInformation(currentSourceContext, 0, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogWarning(Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogWarning(currentSourceContext, 0, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogError(Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogError(currentSourceContext, 0, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogCritical(Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      LogCritical(currentSourceContext, 0, ex);
    }

    #endregion

    #region Ids and MessageTemplate

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogTrace(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(0, currentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogDebug(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(1, currentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogInformation(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(2, currentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogWarning(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(3, currentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogError(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(4, currentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogCritical(long sourceLineId, int kindId, string messageTemplate, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(5, currentSourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    #endregion

    #region Ids and templateEnumElement

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogTrace(long sourceLineId, Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(0, currentSourceContext, sourceLineId, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogDebug(long sourceLineId, Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(1, currentSourceContext, sourceLineId, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogInformation(long sourceLineId, Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(2, currentSourceContext, sourceLineId, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogWarning(long sourceLineId, Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(3, currentSourceContext, sourceLineId, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogError(long sourceLineId, Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(4, currentSourceContext, sourceLineId, templateEnumElement, args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogCritical(long sourceLineId, Enum templateEnumElement, params object[] args) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(5, currentSourceContext, sourceLineId, templateEnumElement, args);
    }

    #endregion

    #region SourceLineId and Exception

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogTrace(long sourceLineId, Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(0, currentSourceContext, sourceLineId, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogDebug(long sourceLineId, Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(1, currentSourceContext, sourceLineId, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogInformation(long sourceLineId, Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(2, currentSourceContext, sourceLineId, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogWarning(long sourceLineId, Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(3, currentSourceContext, sourceLineId, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogError(long sourceLineId, Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(4, currentSourceContext, sourceLineId, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogCritical(long sourceLineId, Exception ex) {
      string currentSourceContext = Assembly.GetCallingAssembly().GetName().Name;
      Log(5, currentSourceContext, sourceLineId, ex);
    }

    #endregion

  }

}
