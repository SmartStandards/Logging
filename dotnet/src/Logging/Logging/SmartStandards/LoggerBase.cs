using System;
using System.Reflection;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Base class to be inherited by channel-specific loggers.
  ///   Implements a built-in default handler, which is pushing messages to System.Diagnostics.Trace.
  /// </summary>
  /// <typeparam name="T"> The type of the channel-specific logger. </typeparam>
  public abstract class LoggerBase<T> {

    private static string _InternalChannelName = null;

    private static string InternalChannelName {
      get {

        if (_InternalChannelName == null) {

          _InternalChannelName = "Dev";

          // We need to use reflection to read the constant because overriding is not possible with static members.
          var fieldInfo = typeof(T).GetField("ChannelName", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

          if (fieldInfo != null) {
            string descendantChannelName = fieldInfo.GetRawConstantValue() as string;

            if (!string.IsNullOrWhiteSpace(descendantChannelName)) _InternalChannelName = descendantChannelName;
          }

        }
        return _InternalChannelName;
      }
    }

    private static Action<string, int, int, string, object[]> _InternalLogMethod;

    /// <summary>
    ///   Hook for injecting external log handler delegates.
    ///   The inherited classes must implement a boilerplate Property "LogMethod" and pass the values from/to here,
    ///   because this is a static class and simple overriding is not possible.
    /// </summary>
    protected static Action<string, int, int, string, object[]> InternalLogMethod {
      get {

        if (_InternalLogMethod is null) _InternalLogMethod = LogToTraceAdapter.LogToTrace;

        return _InternalLogMethod;
      }
      set {
        _InternalLogMethod = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void Log(int level, int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, level, id, messageTemplate, args);
    }

    public static void LogCritical(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, 5, id, messageTemplate, args);
    }

    public static void LogError(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, 4, id, messageTemplate, args);
    }

    public static void LogWarning(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, 3, id, messageTemplate, args);
    }

    public static void LogInformation(int id, string messageTemplate) {
      InternalLogMethod.Invoke(InternalChannelName, 2, id, messageTemplate, null);
    }

    public static void LogInformation(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, 2, id, messageTemplate, args);
    }

    public static void LogDebug(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, 1, id, messageTemplate, args);
    }

    public static void LogTrace(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, 0, id, messageTemplate, args);
    }

    public static void LogReturnCodeAsError(int id, string messageTemplate, object[] args) {

      if (id < 0) {
        LogError(-id, messageTemplate, args);
      } else {
        LogInformation(id, messageTemplate, args);
      }
    }

    public static void LogReturnCodeAsWarning(int id, string messageTemplate, object[] args) {

      if (id < 0) {
        LogWarning(-id, messageTemplate, args);
      } else {
        LogInformation(id, messageTemplate, args);
      }
    }

  }
}
