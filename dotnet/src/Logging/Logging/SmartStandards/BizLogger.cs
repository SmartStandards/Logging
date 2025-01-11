using System;
using System.Logging.SmartStandards;

namespace Logging.SmartStandards {

  public partial class BizLogger {

    public const string AudienceToken = "Biz";

    public const string AudienceTokenU = "BIZ";

    public static void Log(string sourceContext, int level, int id, string messageTemplate, params object[] args) {

      if (Routing.BizLoggerToTraceBus) {
        TraceBusFeed.EmitMessage(sourceContext, AudienceTokenU, level, id, messageTemplate, args);
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(sourceContext, AudienceToken, level, id, messageTemplate, args);
      }
    }

    public static void Log(string sourceContext, int level, Enum logTemplate, params object[] args) {
      LoggingHelper.GetLogTemplateFromEnum(logTemplate, out int id, out string messageTemplate);
      Log(sourceContext, level, id, messageTemplate, args);
    }

    public static void Log(string sourceContext, int level, Exception ex) {

      if (Routing.BizLoggerToTraceBus) {
        TraceBusFeed.EmitException(sourceContext, AudienceTokenU, level, ex);
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(sourceContext, AudienceToken, level, ex);
      }
    }
  }
}