using System;
using Logging.SmartStandards.Internal;
using Logging.SmartStandards.TemplateHousekeeping;
using Logging.SmartStandards.Transport;

namespace Logging.SmartStandards {

  public partial class DevLogger {

    public const string AudienceToken = "Dev";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (messageTemplate == null) messageTemplate = "";

      if (args == null) args = new object[0];

      if (Routing.DevLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, kindId, messageTemplate, args);
      }

      if (Routing.DevLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(AudienceToken, level, sourceContext, sourceLineId, kindId, messageTemplate, args);
      }
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      LogMessageTemplateRepository.GetMessageTemplateByKind(kindEnumElement, out int kindId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {

      int kindId = ExceptionAnalyzer.InferEventIdByException(ex);

      if (Routing.DevLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, kindId, ex);
      }

      if (Routing.DevLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, kindId, ex);
      }
    }
  }
}
