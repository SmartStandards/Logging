using System;
using System.IO;
using System.Text;

namespace Logging.SmartStandards {

  public static class ExceptionRenderer {

    /// <summary>
    ///   Serializes an Exception in a way, that InnerExceptions and StackTraces are included
    ///   and returns a message string, which is highly optimized for logging requirements.
    /// </summary>
    public static string Render(Exception ex, bool includeStacktrace = true) {
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

      target.Append($"-- {ex.GetType().FullName} --");

      if (isInner) {
        target.Append($" (inner)");
      }

      if (includeStacktrace && !string.IsNullOrWhiteSpace(ex.StackTrace)) {

        StringReader traceReader = new StringReader(ex.StackTrace); // TODO: Performance optimization

        string readLine = traceReader.ReadLine()?.Trim();

        while (readLine != null) {
          target.AppendLine();
          target.Append("@ ");
          target.Append(readLine.Replace(" in ", Environment.NewLine + "@   "));
          readLine = traceReader.ReadLine()?.Trim();
        }
      }

      if ((ex.InnerException != null)) {
        messageMainLine = messageMainLine + " :: " + ex.InnerException.Message;
        target.AppendLine();
        AppendRecursive(ex.InnerException, target, ref messageMainLine, includeStacktrace, true);
      }

    }
  }
}
