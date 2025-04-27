using System;
using System.Text;

namespace Logging.SmartStandards {

  public static class ExceptionRenderer {

    /// <summary>
    ///   Serializes an Exception in a way, that InnerExceptions and StackTraces are included
    ///   and returns a message string, which is highly optimized for logging requirements.
    /// </summary>
    public static string Render(Exception ex, bool includeStacktrace = true) {
      try {
        StringBuilder sb = new StringBuilder(1000);
        int messageCursor = 0;
        AppendRecursive(ex, sb, ref messageCursor, includeStacktrace);
        return sb.ToString();

      } catch (Exception ex2) {
        return ex2.ToString();
      }
    }

    private static void AppendRecursive(
      Exception ex, StringBuilder target, ref int messageCursor, bool includeStackTrace, bool isInner = false
    ) {

      if (ex == null) {
        return;
      }

      if (isInner) {
        target.Insert(messageCursor, " :: ");
        messageCursor += 4;
      }

      target.Insert(messageCursor, ex.Message);
      messageCursor += ex.Message.Length;

      if (!isInner) target.Insert(messageCursor, Environment.NewLine);

      target.AppendLine($"-- {ex.GetType().FullName} --");

      string originalStackTrace = ex.StackTrace;

      if (includeStackTrace && !string.IsNullOrWhiteSpace(originalStackTrace)) {

        int cursor = originalStackTrace.Length;
        int lineStartIndex = 0;
        int lineEndIndex = originalStackTrace.Length;

        while (cursor > 0) {
          cursor = originalStackTrace.LastIndexOf(Environment.NewLine, cursor);
          if (cursor < 0) {
            lineStartIndex = 0;
          } else {
            lineStartIndex = cursor + Environment.NewLine.Length;
          }

          // split member / file

          // " in C:\..."
          int fileSegmentStartIndex = originalStackTrace.IndexOf(" in ", lineStartIndex);

          if (fileSegmentStartIndex >= 0) fileSegmentStartIndex += 4;

          int fileSegmentEndIndex = lineEndIndex;

          // "   at MyNamespace..."
          int memberSegmentStartIndex = lineStartIndex + 6;

          int memberSegmentEndIndex = (fileSegmentStartIndex >= 0) ? fileSegmentStartIndex - 4 : lineEndIndex;

          target.Append("@ ");
          target.Append(originalStackTrace, memberSegmentStartIndex, memberSegmentEndIndex - memberSegmentStartIndex);
          target.AppendLine();

          if (fileSegmentStartIndex >= 0) {
            target.Append("@   ");
            target.Append(originalStackTrace, fileSegmentStartIndex, fileSegmentEndIndex - fileSegmentStartIndex);
            target.AppendLine();
          }

          lineEndIndex = cursor;
        }

      }

      if ((ex.InnerException != null)) {
        AppendRecursive(ex.InnerException, target, ref messageCursor, includeStackTrace, true);
      }

    }
  }
}
