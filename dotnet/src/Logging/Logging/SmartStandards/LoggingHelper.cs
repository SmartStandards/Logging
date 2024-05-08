using System.Text;

namespace Logging.SmartStandards {

  public static class LoggingHelper {

    public static string CreateFormattedLogError(string statusMessage) {
      return StatusToFormattedLogEntry(4, 0, statusMessage, null);
    }

    public static string StatusToFormattedLogEntry(int level, int returnCode, string statusMessageTemplate, object[] statusMessageArgs) {

      StringBuilder sb = new StringBuilder((int)(statusMessageTemplate.Length * 1.5));


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
        sb.TryResolveArgs(statusMessageArgs, startIndex);
      }

      return sb.ToString();
    }

  }
}
