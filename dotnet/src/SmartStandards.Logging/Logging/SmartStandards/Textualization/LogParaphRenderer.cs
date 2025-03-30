using System.Text;

namespace Logging.SmartStandards.Textualization {

  /// <summary>
  ///   Tool to build textual representations of log events.
  /// </summary>
  /// <remarks>
  ///   Example LogParaph:
  ///   [LevelAsAlpha3] SourceContext #EventId#  SourceLineId [AudienceToken]: MessageTemplate 
  ///   [Err] MyApp.exe #4711# 2070198253252296432 [Ins]: File not found on Disk! 
  /// </remarks>
  public class LogParaphRenderer {

    public static string LevelToAlpha3(int level, StringBuilder targetStringBuilder = null) {

      switch (level) {

        case 5: { // Critical
          targetStringBuilder?.Append("Cri");
          return "Cri";
        }
        case 4: { // Error
          targetStringBuilder?.Append("Err");
          return "Err";
        }
        case 3: { // Warning
          targetStringBuilder?.Append("Wrn");
          return "Wrn";
        }
        case 2: { // Information
          targetStringBuilder?.Append("Inf");
          return "Inf";
        }
        case 1: { // Debug
          targetStringBuilder?.Append("Dbg");
          return "Dbg";
        }
        default: { // Trace
          targetStringBuilder?.Append("Trc");
          return "Trc";
        }
      }
    }

    /// <summary>
    ///   Appends a ready-to-read log paraph (having resolved placeholders) to an existing StringBuilder instance.
    /// </summary>    
    /// <returns>
    ///   Example LogParaph:
    ///   [LevelAsAlpha3] SourceContext #EventId#  SourceLineId [AudienceToken]: MessageTemplate 
    ///   [Err] MyApp.exe #4711# 2070198253252296432 [Ins]: File not found on Disk! 
    /// </returns>
    public static StringBuilder BuildParaphResolved(
      StringBuilder targetStringBuilder,
      string audienceToken, int level, string sourceContext, long sourceLineId,
      int eventId, string messageTemplate, object[] args
    ) {

      LogParaphRenderer.BuildParaphLeftPart(targetStringBuilder, level, sourceContext, eventId);

      LogParaphRenderer.BuildParaphRightPart(targetStringBuilder, sourceLineId, audienceToken, null);

      targetStringBuilder.AppendResolved(messageTemplate, args);

      return targetStringBuilder;
    }

    /// <summary>
    ///   Renders the left part of a log paraph.
    /// </summary>
    /// <remarks>
    ///   The left part contains meta data that would normally be transported as arguments.
    /// </remarks>
    /// <returns>
    ///   S.th. like "[Err] MyApp.exe #4711#"
    /// </returns>
    public static StringBuilder BuildParaphLeftPart(
      StringBuilder targetStringBuilder, int level, string sourceContext, int eventId
    ) {
      targetStringBuilder.Append('[');
      LogParaphRenderer.LevelToAlpha3(level, targetStringBuilder);
      targetStringBuilder.Append("] ");
      targetStringBuilder.Append(sourceContext);
      targetStringBuilder.Append(" #");
      targetStringBuilder.Append(eventId);
      targetStringBuilder.Append('#');
      return targetStringBuilder;
    }

    /// <summary>
    ///   Renders the right part of a log paraph.
    /// </summary>
    /// <remarks>
    ///   The right part contains meta data that cannot be transported as arguments, because most logging APIs do not offer enough
    ///   parameters.
    /// </remarks>
    /// <returns>
    ///   S.th. like " 2070198253252296432 [Ins]: File not found on Disk! "
    /// </returns>
    public static StringBuilder BuildParaphRightPart(
      StringBuilder targetStringBuilder, long sourceLineId, string audienceToken, string messageTemplate
    ) {
      targetStringBuilder.Append(' ');
      targetStringBuilder.Append(sourceLineId);
      targetStringBuilder.Append(" [");
      targetStringBuilder.Append(audienceToken);
      targetStringBuilder.Append("]: ");
      targetStringBuilder.Append(messageTemplate);
      return targetStringBuilder;
    }

  }
}

