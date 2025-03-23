using System;
using System.Logging.SmartStandards;

namespace Logging.SmartStandards {

  public partial class DevLogger {

    public const string AudienceToken = "Dev";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, params object[] args
    ) {

      if (Routing.DevLoggerToTraceBus) {
        TraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
      }

      if (Routing.DevLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
      }
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Enum logTemplate, params object[] args) {
      LoggingHelper.GetLogTemplateFromEnum(logTemplate, out int eventId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, eventId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {

      if (Routing.DevLoggerToTraceBus) {
        TraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, ex);
      }

      if (Routing.DevLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, ex);
      }
    }
  }
}