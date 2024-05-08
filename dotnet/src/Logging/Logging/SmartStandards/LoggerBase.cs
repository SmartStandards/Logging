using System;
using System.Diagnostics;
using System.Reflection;

namespace Logging.SmartStandards {

  /// <summary>
  ///   [v 1.0.0] Base class to be inherited by channel-specific loggers.
  ///   Implements a built-in default handler, which is pushing messages to System.Diagnostics.Trace.
  /// </summary>
  /// <typeparam name="T"> The type of the channel-specific logger. </typeparam>
  public abstract class LoggerBase<T> {

    #region Base Members

    private static Action<int, int, string, object[]> _InternalLogMethod;

    /// <summary>
    ///   Hook for injecting external log handler delegates.
    ///   The inherited classes must implement a boilerplate Property "LogMethod" and pass the values from/to here,
    ///   because this is a static class and simple overriding is not possible.
    /// </summary>
    protected static Action<int, int, string, object[]> InternalLogMethod {
      get {
        if (_InternalLogMethod is null) {
          _InternalLogMethod = DefaultLogMethod;
        }
        return _InternalLogMethod;
      }
      set {
        _InternalLogMethod = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void Log(int level, int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(level, id, messageTemplate, args);
    }

    public static void LogCritical(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(5, id, messageTemplate, args);
    }

    public static void LogError(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(4, id, messageTemplate, args);
    }

    public static void LogWarning(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(3, id, messageTemplate, args);
    }

    public static void LogInformation(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(2, id, messageTemplate, args);
    }

    public static void LogDebug(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(1, id, messageTemplate, args);
    }

    public static void LogTrace(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(0, id, messageTemplate, args);
    }

    #endregion

    #region Convenience Methods for logging return codes + status messages

    public static void LogReturnCodeAsError(int id, string messageTemplate, object[] args) {

      if (messageTemplate is null)
        return;

      if (id < 0) {
        LogError(-id, messageTemplate, args);
      } else {
        LogInformation(id, messageTemplate, args);
      }
    }

    public static void LogReturnCodeAsWarning(int id, string messageTemplate, object[] args) {

      if (messageTemplate is null)
        return;

      if (id < 0) {
        LogWarning(-id, messageTemplate, args);
      } else {
        LogInformation(id, messageTemplate, args);
      }
    }

    #endregion

    #region Implementation of the built-in default handler

    private static TraceSource _InnerTraceSource;

    private static TraceSource InnerTraceSource {
      get {
        if (_InnerTraceSource is null) { // Lazily wire up the trace source...

          // ... but ensure the desired listener was initialized before:

          bool found = false;

          foreach (TraceListener l in Trace.Listeners) {
            if (l.GetType().Name.EndsWith("TraceLogPipe")) { // convention!
              found = true;
              break;
            }
          }

          if (!found) return null;

          // actual wire-up

          string channelName = "DEV"; // Fallback channel name (in case the inherited class didn't implement the "ChannelName" constant.

          // We need to use reflection to read the constant because overriding is not possible with static members.
          var fieldInfo = typeof(T).GetField("ChannelName", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

          if (fieldInfo != null) {
            string descendantChannelName = fieldInfo.GetRawConstantValue() as string;

            if (!string.IsNullOrWhiteSpace(descendantChannelName)) channelName = descendantChannelName;
          }

          _InnerTraceSource = new TraceSource(channelName);
          _InnerTraceSource.Switch.Level = SourceLevels.All;
          _InnerTraceSource.Listeners.Clear(); // Otherwise the default listener will be registered twice
          _InnerTraceSource.Listeners.AddRange(Trace.Listeners); // Wire up all CURRENTLY existing listeners (they have to initialized before!)
        }
        return _InnerTraceSource;
      }
    }

    private static void DefaultLogMethod(int level, int id, string messageTemplate, object[] args) {

      if (InnerTraceSource is null)
        return;

      TraceEventType eventType;

      switch (level) {

        case 5: { // Critical (aka "Fatal")
          eventType = TraceEventType.Critical; // 1
          break;
        }

        case 4: { // Error
          eventType = TraceEventType.Error; // 2
          break;
        }

        case 3: { // Warning
          eventType = TraceEventType.Warning; // 4
          break;
        }

        case 2: { // Info
          eventType = TraceEventType.Information; // 8
          break;
        }

        case 1: { // Debug
          eventType = TraceEventType.Transfer; // 4096 - ' There is no "Debug" EventType => use something else
                                               // 0 "Trace" (aka "Verbose")
          break;
        }

        default: {
          eventType = TraceEventType.Verbose; // 16
          break;
        }

      }

      // Because we support named placeholders (like "Hello {audience}") instead of old scool indexed place holders (like "Hello {0}")
      // we need to double brace the placeholders - otherwise there will be exceptions coming from the .net TraceEvent Method.

      string formatString = messageTemplate?.Replace("{", "{{").Replace("}", "}}");

      if (id < 0) id = -id;

      // Normalize the transport of empty messages - Prefer null over empty string/array.
      // Remember: ParamArray will implicitely create an empty array if no arguments are passed.

      if (string.IsNullOrWhiteSpace(formatString)) formatString = null;

      if (args != null && args.Length == 0) args = null;

      InnerTraceSource.TraceEvent(eventType, id, formatString, args);

    }

    #endregion

  }

}
