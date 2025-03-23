using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Logging.SmartStandards {

  public static class ExceptionSerializer {

    /// <summary>
    ///   Serializes an Exception in a way, that InnerExceptions and StackTraces are included
    ///   and returns a message string, which is highly optimized for logging requirements.
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="includeStacktrace"></param>
    /// <returns></returns>
    public static string Serialize(this Exception ex, bool includeStacktrace = true) { // todo serialize
      StringBuilder sb = new StringBuilder(1000);
      string messageMainLine = ex.Message;
      AppendRecursive(ex, sb, ref messageMainLine, includeStacktrace);
      sb.Insert(0, messageMainLine + Environment.NewLine);
      return sb.ToString();
    }

    private static void AppendRecursive(Exception ex, StringBuilder target, ref string messageMainLine, bool includeStacktrace, bool isInner = false) {

      if (ex == null) {
        return;
      }

      target.Append($"__{ex.GetType().FullName}__");
      if (isInner) {
        target.Append($" (inner)");
      }

      if (includeStacktrace && !string.IsNullOrWhiteSpace(ex.StackTrace)) {
        var tr = new StringReader(ex.StackTrace);
        string currentStacktraceLine = tr.ReadLine()?.Trim();
        while (currentStacktraceLine != null) {
          target.AppendLine();
          target.Append("@   ");
          target.Append(currentStacktraceLine.Replace(" in ", Environment.NewLine + "@   "));
          currentStacktraceLine = tr.ReadLine()?.Trim();
        }
      }

      if ((ex.InnerException != null)) {
        messageMainLine = messageMainLine + " >> " + ex.InnerException.Message;
        target.AppendLine();
        AppendRecursive(ex.InnerException, target, ref messageMainLine, includeStacktrace, true);
      }

    }

    internal static int GetGenericIdFromException(Exception ex) {
      if (ex is TargetInvocationException && ex.InnerException != null) {
        return GetGenericIdFromException(ex.InnerException);
      }
      if (ex is Win32Exception) {
        return ((Win32Exception)ex).NativeErrorCode;
      }
      int hashTagIndex = ex.Message.LastIndexOf("#");
      if (hashTagIndex >= 0 && int.TryParse(ex.Message.Substring(hashTagIndex + 1), out int id)) {
        return id;
      } else {
        using (var md5 = MD5.Create()) {
          int hash = BitConverter.ToInt32(md5.ComputeHash(Encoding.UTF8.GetBytes(ex.GetType().Name)), 0);
          if (hash < 0) {
            return hash * -1;
          }
          return hash;
        }
      }
    }
  }

}
