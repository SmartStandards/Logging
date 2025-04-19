using System;
using Logging.SmartStandards.Internal;
using Logging.SmartStandards.TemplateHousekeeping;
using Logging.SmartStandards.Transport;

namespace Logging.SmartStandards {

  public partial class InsLogger {

    public const string AudienceToken = "Ins";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (messageTemplate == null) messageTemplate = "";

      if (args == null) args = new object[0];

      if (Routing.InsLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, kindId, messageTemplate, args);
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(AudienceToken, level, sourceContext, sourceLineId, kindId, messageTemplate, args);
      }
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Enum templateEnumElement, params object[] args) {
      LogEventTemplateRepository.GetLogEventTemplateByEnum(templateEnumElement, out int kindId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {

      int kindId = ExceptionAnalyzer.InferEventKindByException(ex);

      if (Routing.InsLoggerToTraceBus) {
        if (Routing.TraceBusExceptionsTextualizedToggle) {
          string renderedException = ExceptionRenderer.Render(ex);
          Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, kindId, renderedException);
        } else {
          Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, kindId, ex);
        }
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, kindId, ex);
      }
    }
  }
}
