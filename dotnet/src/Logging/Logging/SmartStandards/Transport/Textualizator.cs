namespace Logging.SmartStandards.Transport {

  internal class Textualizator {

    public static void TokenizeMetaDataRightPart(
       string formatString, out long sourceLineId, out string audienceToken, out string messageTemplate
     ) {

      sourceLineId = 0;
      audienceToken = "???";
      messageTemplate = formatString;

      // " SourceLineId [AudienceToken]: MessageTemplate"
      //  01234567890
      // " 0 [Ins]: MessageTemplate"

      if (formatString == null || formatString.Length < 9) return;

      if (formatString[0] != ' ') return;

      int rightOfSourceLineId = formatString.IndexOf(' ', 1);
      int leftOfAudience = formatString.IndexOf('[');
      int rightOfAudience = formatString.IndexOf("]:");

      if (
        rightOfSourceLineId > leftOfAudience ||
        leftOfAudience > rightOfAudience ||
        rightOfAudience - leftOfAudience != 4
      ) return;

      string sourceLineIdAsString = formatString.Substring(1, rightOfSourceLineId);

      if (!long.TryParse(sourceLineIdAsString, out sourceLineId)) return;

      audienceToken = formatString.Substring(leftOfAudience + 1, 3);

      int beginOfMessageTemplate = rightOfAudience + 3;

      if (formatString.Length >= beginOfMessageTemplate) {
        messageTemplate = formatString.Substring(beginOfMessageTemplate);
      } else {
        messageTemplate = "";
      }
    }

  }

}
