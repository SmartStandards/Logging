using Logging.SmartStandards.Internal;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Logging.SmartStandards {

  public class ReturnCodeLogger {

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
      if (returnCode <= -500000000) {
        return 5;// Critical
      } else if (returnCode < 0) {
        return 4; // Error
      }
      return 2; // Info
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

  }
}
