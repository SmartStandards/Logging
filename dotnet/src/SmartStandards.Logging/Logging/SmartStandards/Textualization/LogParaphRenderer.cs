using System.Collections.Generic;
using System.Text;

namespace Logging.SmartStandards.Textualization {

  /// <summary>
  ///   Tool to build textual representations of log events.
  /// </summary>
  /// <remarks>
  ///   Example LogParaph:
  ///   [LevelAsAlpha3] SourceContext #eventKindId#  SourceLineId [AudienceToken]: MessageTemplate 
  ///   [Err] MyApp.exe #4711# 2070198253252296432 [Ins]: File not found on Disk! 
  /// </remarks>
  public partial class LogParaphRenderer { // v 1.0.0

    /// <summary>
    ///   Appends a ready-to-read log paraph (having resolved placeholders) to an existing StringBuilder instance.
    /// </summary>    
    /// <returns>
    ///   Example LogParaph:
    ///   [LevelAsAlpha3] SourceContext #eventKindId#  SourceLineId [AudienceToken]: MessageTemplate 
    ///   [Err] MyApp.exe #4711# 2070198253252296432 [Ins]: File not found on Disk! 
    /// </returns>
    public static StringBuilder BuildParaphResolved(
      StringBuilder targetStringBuilder,
      string audienceToken, int level, string sourceContext, long sourceLineId,
      int eventKindId, string messageTemplate, IDictionary<string, string> fields
    ) {

      LogParaphRenderer.BuildParaphLeftPart(targetStringBuilder, level, sourceContext, eventKindId);

      LogParaphRenderer.BuildParaphRightPart(targetStringBuilder, sourceLineId, audienceToken, null);

      targetStringBuilder.AppendResolving(messageTemplate, (placeholder) => {
        if (fields != null && fields.TryGetValue(placeholder, out string value)) {
          return value;
        }
        return null;
      });

      return targetStringBuilder;
    }

  }
}

