using Logging.SmartStandards.Internal;
using Logging.SmartStandards.Textualization;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Convenience Class to simplify the logging of return codes and status messages coming from function calls.
  ///   Return code will be mapped to error level like this:
  ///     0 and above to Trace
  ///     below -300000000 to Error
  ///     below -400000000 to Critical
  /// </summary>
  /// <remarks>
  ///   Suggested usage: Do not use this for permanent solutions - instead implement propper logging per return code.
  ///   Do use this to quickly (and temporarily) add tracing of a function call into your code.
  /// </remarks>
  public static class ReturnCodeLogger {

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void DevLog(long sourceLineId, string calledMethodName, int returnCode, string messageTemplate, object[] args) {

      string currentSourceContext = SourceContextDiscoverer.InferSourceContext(Assembly.GetCallingAssembly());

      DevLogger.Log(
        InferLevel(returnCode), currentSourceContext, sourceLineId,
        0, ComposeMessageTemplate(messageTemplate), ComposeArgs(returnCode, calledMethodName, args)
      );
    }

    public static void DevLog(string sourceContext, long sourceLineId, string calledMethodName, int returnCode, string messageTemplate, object[] args) {

      DevLogger.Log(
        InferLevel(returnCode), sourceContext, sourceLineId,
        0, ComposeMessageTemplate(messageTemplate), ComposeArgs(returnCode, calledMethodName, args)
      );
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InsLog(long sourceLineId, string calledMethodName, int returnCode, string messageTemplate, object[] args) {

      string currentSourceContext = SourceContextDiscoverer.InferSourceContext(Assembly.GetCallingAssembly());

      InsLogger.Log(
        InferLevel(returnCode), currentSourceContext, sourceLineId,
        0, ComposeMessageTemplate(messageTemplate), ComposeArgs(returnCode, calledMethodName, args)
      );

    }

    public static void InsLog(string sourceContext, long sourceLineId, string calledMethodName, int returnCode, string messageTemplate, object[] args) {

      InsLogger.Log(
        InferLevel(returnCode), sourceContext, sourceLineId,
        0, ComposeMessageTemplate(messageTemplate), ComposeArgs(returnCode, calledMethodName, args)
      );

    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void BizLog(long sourceLineId, string calledMethodName, int returnCode, string messageTemplate, object[] args) {

      string currentSourceContext = SourceContextDiscoverer.InferSourceContext(Assembly.GetCallingAssembly());

      BizLogger.Log(
        InferLevel(returnCode), currentSourceContext, sourceLineId,
        0, ComposeMessageTemplate(messageTemplate), ComposeArgs(returnCode, calledMethodName, args)
      );

    }

    public static void BizLog(string sourceContext, long sourceLineId, string calledMethodName, int returnCode, string messageTemplate, object[] args) {

      BizLogger.Log(
        InferLevel(returnCode), sourceContext, sourceLineId,
        0, ComposeMessageTemplate(messageTemplate), ComposeArgs(returnCode, calledMethodName, args)
      );

    }

    private static int InferLevel(int returnCode) {
      if (returnCode <= -400000000) {
        return 5;// Critical
      } else if (returnCode <= -300000000) {
        return 4; // Error
      }
      return 0; // Trace
    }

    private static string ComposeMessageTemplate(string statusMessageTemplate) {
      return "ReturnCode {ReturnCodeToBeLogged} from {CalledMethodToBeLogged}: " + statusMessageTemplate;
    }

    private static object[] ComposeArgs(int returnCode, string calledMethodName, object[] args) {

      int l = 2;

      if (args != null) l += args.Length;

      object[] composedArgs = new object[l];
      composedArgs[0] = returnCode;
      composedArgs[1] = calledMethodName;

      if (args != null) Array.Copy(args, 0, composedArgs, 2, args.Length);

      return composedArgs;
    }

    public static void AppendReturnCode(this StringBuilder builder, string calledMethodName, int returnCode, string statusMessageTemplate, object[] statusMessageArgs) {
      builder.Append("ReturnCode ").Append(returnCode).Append(" from ").Append(calledMethodName).Append(": ");
      builder.AppendResolved(statusMessageTemplate, statusMessageArgs);
    }

    public static string Textualize(string calledMethodName, int returnCode, string statusMessageTemplate, object[] statusMessageArgs) {

      StringBuilder builder = new StringBuilder(1024);

      builder.AppendReturnCode(calledMethodName, returnCode, statusMessageTemplate, statusMessageArgs);

      return builder.ToString();
    }

  }
}
