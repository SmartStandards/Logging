using System.SmartStandards;
using System.Text;

namespace Logging.SmartStandards {

  public static class LoggingHelper {

    public static string CreateFormattedLogError(string statusMessage) {
      return StatusToFormattedLogEntry(4, 0, statusMessage, null);
    }

    /// <summary>
    ///   Renders a "Formatted Log Entry" (see conventions) from a statusMessageTemplate + statusMessageArgs combo.
    ///   The syntax is: "[LevelAsAlpha3]Id:ResolvedMessage"
    /// </summary>
    /// <param name="level">
    ///   The numeric level of the status message. Will be rendered as Alpha3 representation:
    ///   0: "Trc"
    ///   1: "Dbg"
    ///   2: "Inf"
    ///   3: "Wrn"
    ///   4: "Err"
    ///   5 : "Cri" (will be used as fallback for all other values)
    /// </param>
    /// <param name="returnCode"> The passed return code will be used as id. </param>
    /// <param name="statusMessageTemplate"> 
    ///   The statusMessageTemplate, the placeholders of which will be resolved.
    ///   null is equivalent to passing an empty string (the statusMessageArgs are ignored).
    /// </param>
    /// <param name="statusMessageArgs">
    ///   The values that are used for placeholder resolving.
    ///   null is equivalent to passing an empty array.
    /// </param>
    /// <returns> A string like "[Inf]4711:Hello world, the answer is 42.". </returns>
    public static string StatusToFormattedLogEntry(int level, int returnCode, string statusMessageTemplate, object[] statusMessageArgs) {

      int assumedLength;

      if (statusMessageTemplate != null) {
        assumedLength = (int)(statusMessageTemplate.Length * 1.5);
      } else {
        assumedLength = 10;
      }

      StringBuilder sb = new StringBuilder(assumedLength);

      // "[ERR]4711:File not found on Disk."

      sb.Append('[');

      switch (level) {
        case 0: { sb.Append("Trc"); break; }
        case 1: { sb.Append("Dbg"); break; }
        case 2: { sb.Append("Inf"); break; }
        case 3: { sb.Append("Wrn"); break; }
        case 4: { sb.Append("Err"); break; }
        default: { sb.Append("Cri"); break; }
      }

      sb.Append(']');
      if (returnCode != 0) { sb.Append(returnCode); }
      sb.Append(':');

      if (statusMessageTemplate != null && statusMessageTemplate.Length > 0) {
        int startIndex = sb.Length;
        sb.Append(statusMessageTemplate);
        sb.ResolvePlaceholders(statusMessageArgs);
      }

      return sb.ToString();
    }

  }
}
