namespace Logging.SmartStandards.Textualization {

  public class LogParaphParser {

    /// <summary>
    ///   Tokenizes an incoming metaDataRightPart string.
    /// </summary>
    /// <param name="metaDataRightPart">
    ///   " SourceLineId [AudienceToken]: MessageTemplate"
    /// </param>
    /// <param name="sourceLineId"> If metaDataRightPart was malformed: 0. </param>
    /// <param name="audienceToken"> Without the brackets. If metaDataRightPart was malformed: ???. </param>
    /// <param name="messageTemplate"> If metaDataRightPart was malformed: metaDataRightPart.</param>
    public static void TokenizeMetaDataRightPart(
      string metaDataRightPart, out long sourceLineId, out string audienceToken, out string messageTemplate
    ) {

      sourceLineId = 0;
      audienceToken = "???";
      messageTemplate = metaDataRightPart;

      if (metaDataRightPart == null || metaDataRightPart.Length < 9) return;

      if (metaDataRightPart[0] != ' ') return;

      int rightOfSourceLineId = metaDataRightPart.IndexOf(' ', 1);
      int leftOfAudience = metaDataRightPart.IndexOf('[');
      int rightOfAudience = metaDataRightPart.IndexOf("]:");

      if (
        rightOfSourceLineId > leftOfAudience ||
        leftOfAudience > rightOfAudience ||
        rightOfAudience - leftOfAudience != 4
      ) return;

      string sourceLineIdAsString = metaDataRightPart.Substring(1, rightOfSourceLineId);

      if (!long.TryParse(sourceLineIdAsString, out sourceLineId)) return;

      audienceToken = metaDataRightPart.Substring(leftOfAudience + 1, 3);

      int beginOfMessageTemplate = rightOfAudience + 3;

      if (metaDataRightPart.Length >= beginOfMessageTemplate) {
        messageTemplate = metaDataRightPart.Substring(beginOfMessageTemplate);
      } else {
        messageTemplate = "";
      }
    }
  }
}
