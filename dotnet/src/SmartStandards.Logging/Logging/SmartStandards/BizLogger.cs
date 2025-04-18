using System;
using Logging.SmartStandards.Internal;
using Logging.SmartStandards.TemplateHousekeeping;
using Logging.SmartStandards.Transport;

namespace Logging.SmartStandards {

  public partial class BizLogger {

    public const string AudienceToken = "Biz";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (messageTemplate == null) messageTemplate = "";

      if (args == null) args = new object[0];

      if (Routing.BizLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, kindId, messageTemplate, args);
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(AudienceToken, level, sourceContext, sourceLineId, kindId, messageTemplate, args);
      }
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      LogMessageTemplateRepository.GetMessageTemplateByKind(kindEnumElement, out int kindId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {

      int kindId = ExceptionAnalyzer.InferEventKindByException(ex);

      if (Routing.BizLoggerToTraceBus) {
        if (Routing.TraceBusExceptionsTextualizedToggle) {
          string renderedException = ExceptionRenderer.Render(ex);
          Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, kindId, renderedException);
        } else {
          Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, kindId, ex);
        }
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, kindId, ex);
      }
    }
  }
}
