using System;
using System.Linq;
using System.Reflection;

namespace Logging.SmartStandards {

  public delegate void LogMethod(string source, int level, int eventId, string messageTemplate, object[] args);
  public delegate void LogExceptionMethod(string source, int level, int eventId, Exception ex);
  public delegate void InternalLogMethod(string source, bool receivedViaTrace, int level, int eventId, string messageTemplate, object[] args);
  public delegate void InternalLogExceptionMethod(string source, bool receivedViaTrace, int level, int eventId, Exception ex);

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

    private static InternalLogMethod _InternalLogMethod;
    private static InternalLogExceptionMethod _InternalExceptionLogMethod;

    internal static bool IsRedirected {
      get {
        return (_InternalLogMethod != null);
      }
    }

    private static bool _AwaitsInputFromTracing = false;
    public static bool AwaitsInputFromTracing {
      get {
        return _AwaitsInputFromTracing;
      }
      protected set {
        _AwaitsInputFromTracing = value;
      }
    }

    /// <summary>
    ///   Hook for injecting external log handler delegates.
    ///   The inherited classes must implement a boilerplate Property "LogMethod" and pass the values from/to here,
    ///   because this is a static class and simple overriding is not possible.
    /// </summary>
    internal static InternalLogMethod InternalLogMethod {
      get {
        if (_InternalLogMethod == null) {
          return DefaultLogToTraceMethod;
        } 
        return _InternalLogMethod;
      }
      set {
        _InternalLogMethod = value;
      }
    }

    protected static void DefaultLogToTraceMethod(string chnl, bool receivedViaTrace, int lvl, int id, string messageTemplate, object[] args) {
      if (!receivedViaTrace) {
        LogToTraceAdapter.LogToTrace(chnl, lvl, id, messageTemplate, args);
      }
    }

    internal static InternalLogExceptionMethod InternalExceptionLogMethod {
      get {
        if (_InternalExceptionLogMethod == null) {
          return FallbackExceptionLogMethod;
        }
        return _InternalExceptionLogMethod;
      }
      set {
        _InternalExceptionLogMethod = value;
      }
    }

    private static void FallbackExceptionLogMethod(string chnl, bool receivedViaTrace, int lvl,int id, Exception ex) {
      string serializedException = ex.Serialize();
      InternalLogMethod.Invoke(chnl, receivedViaTrace, lvl, id, serializedException, new object[] { ex });
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void Log(int level, int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, false, level, id, messageTemplate, args);
    }

    public static void LogCritical(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, false, 5, id, messageTemplate, args);
    }
    public static void LogCritical(int id, Exception ex) {
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 5, id, ex);
    }
    public static void LogCritical(Exception ex) {
      int id = ExceptionSerializer.GetGenericIdFromException(ex);
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 5, id, ex);
    }

    public static void LogError(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, false, 4, id, messageTemplate, args);
    }
    public static void LogError(int id, Exception ex) {
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 4, id, ex);
    }
    public static void LogError(Exception ex) {
      int id = ExceptionSerializer.GetGenericIdFromException(ex);
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 4, id, ex);
    }

    public static void LogWarning(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, false, 3, id, messageTemplate, args);
    }
    public static void LogWarning(int id, Exception ex) {
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 3, id, ex);
    }
    public static void LogWarning(Exception ex) {
      int id = ExceptionSerializer.GetGenericIdFromException(ex);
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 3, id, ex);
    }

    public static void LogInformation(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, false, 2, id, messageTemplate, args);
    }
    public static void LogInformation(int id, Exception ex) {
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 2, id, ex);
    }
    public static void LogInformation(Exception ex) {
      int id = ExceptionSerializer.GetGenericIdFromException(ex);
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 2, id, ex);
    }

    public static void LogDebug(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, false, 1, id, messageTemplate, args);
    }
    public static void LogDebug(int id, Exception ex) {
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 1, id, ex);
    }
    public static void LogDebug(Exception ex) {
      int id = ExceptionSerializer.GetGenericIdFromException(ex);
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 1, id, ex);
    }

    public static void LogTrace(int id, string messageTemplate, params object[] args) {
      InternalLogMethod.Invoke(InternalChannelName, false, 0, id, messageTemplate, args);
    }
    public static void LogTrace(int id, Exception ex) {
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 0, id, ex);
    }
    public static void LogTrace(Exception ex) {
      int id = ExceptionSerializer.GetGenericIdFromException(ex);
      InternalExceptionLogMethod.Invoke(InternalChannelName, false, 0, id, ex);
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

    internal const string MirrorMarker = "**MIRROR**";
    protected static object[] MirrorArgArray = new object[] { MirrorMarker };
    protected static object[] ConcatMirrorArgArray(object[] args) {
      if (args == null || args.Length == 0) {
        return MirrorArgArray;
      }
      else {
        return args.Concat(MirrorArgArray).ToArray();
      }
    }

  }

}
