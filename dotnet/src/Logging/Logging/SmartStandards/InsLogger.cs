using System;
using System.Logging.SmartStandards;

namespace Logging.SmartStandards {

  public partial class InsLogger {

    public const string AudienceToken = "Ins";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, params object[] args
    ) {

      if (Routing.InsLoggerToTraceBus) {
        TraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
      }
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Enum logTemplate, params object[] args) {
      LoggingHelper.GetLogTemplateFromEnum(logTemplate, out int eventId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, eventId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {

      if (Routing.InsLoggerToTraceBus) {
        TraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, ex);
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, ex);
      }
    }
  }
}