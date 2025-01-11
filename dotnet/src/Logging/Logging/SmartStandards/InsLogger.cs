using System;
using System.Logging.SmartStandards;

namespace Logging.SmartStandards {

  public partial class InsLogger {

    public const string AudienceToken = "Ins";

    public const string AudienceTokenU = "INS";

    public static void Log(string sourceContext, int level, int id, string messageTemplate, params object[] args) {

      if (Routing.InsLoggerToTraceBus) {
        TraceBusFeed.EmitMessage(sourceContext, AudienceTokenU, level, id, messageTemplate, args);
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(sourceContext, AudienceToken, level, id, messageTemplate, args);
      }
    }

    public static void Log(string sourceContext, int level, Enum logTemplate, params object[] args) {
      LoggingHelper.GetLogTemplateFromEnum(logTemplate, out int id, out string messageTemplate);
      Log(sourceContext, level, id, messageTemplate, args);
    }

    public static void Log(string sourceContext, int level, Exception ex) {

      if (Routing.InsLoggerToTraceBus) {
        TraceBusFeed.EmitException(sourceContext, AudienceTokenU, level, ex);
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(sourceContext, AudienceToken, level, ex);
      }
    }
  }
}