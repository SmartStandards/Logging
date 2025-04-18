using System.Text;

namespace Logging.SmartStandards.Textualization {

  // This is the minimum portion of LogParaphRenderer (which is included by SmartStandards.Logging.tt)

  public partial class LogParaphRenderer {

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
