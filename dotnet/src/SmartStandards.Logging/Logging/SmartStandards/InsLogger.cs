using Logging.SmartStandards.Internal;
using Logging.SmartStandards.Transport;
using Logging.SmartStandards.UseCaseManagement;
using System;

namespace Logging.SmartStandards {

  public partial class InsLogger {

    public const string AudienceToken = "Ins";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int useCaseId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (messageTemplate == null) messageTemplate = "";

      if (args == null) args = new object[0];

#if !UsedByT4
      if (Routing.InsLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, useCaseId, messageTemplate, args);
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(AudienceToken, level, sourceContext, sourceLineId, useCaseId, messageTemplate, args);
      }
#else
      TraceBusFeed.Instance.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, useCaseId, messageTemplate, args);
#endif
    }

    public static void Log(
      int level, string sourceContext, long sourceLineId, Enum useCaseEnumElement, params object[] args
    ) {
      LogUseCaseRepository.GetUseCaseIdAndMessageTemplateByEnum(useCaseEnumElement, out int useCaseId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, useCaseId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {

      int useCaseId = ExceptionAnalyzer.InferUseCaseIdByException(ex);

#if !UsedByT4
      if (Routing.InsLoggerToTraceBus) {
        if (Routing.TraceBusExceptionsTextualizedToggle) {
          string renderedException = ExceptionRenderer.Render(ex);
          Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, useCaseId, renderedException);
        } else {
          Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, useCaseId, ex);
        }
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, useCaseId, ex);
      }
#else
      TraceBusFeed.Instance.EmitException(AudienceToken, level, sourceContext, sourceLineId, useCaseId, ex); 
#endif
    }

    public static void Log(int level, string sourceContext, long sourceLineId, int useCaseId, Exception ex) {

#if !UsedByT4
      if (Routing.InsLoggerToTraceBus) {
        if (Routing.TraceBusExceptionsTextualizedToggle) {
          string renderedException = ExceptionRenderer.Render(ex);
          Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, useCaseId, renderedException);
        } else {
          Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, useCaseId, ex);
        }
      }

      if (Routing.InsLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, useCaseId, ex);
      }
#else
      TraceBusFeed.Instance.EmitException(AudienceToken, level, sourceContext, sourceLineId, useCaseId, ex); 
#endif
    }

  }
}
