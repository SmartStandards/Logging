using System;
using Logging.SmartStandards.EventKindManagement;
using Logging.SmartStandards.Internal;
using Logging.SmartStandards.Transport;

namespace Logging.SmartStandards {

  public partial class BizLogger {

    public const string AudienceToken = "Biz";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (messageTemplate == null) messageTemplate = "";

      if (args == null) args = new object[0];

#if !UsedByT4
      if (Routing.BizLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
      }
#else
      TraceBusFeed.Instance.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
#endif
    }

    public static void Log(
      int level, string sourceContext, long sourceLineId, Enum eventKindEnumElement, params object[] args
    ) {
      EventKindRepository.GetKindIdAndMessageTemplateByEnum(eventKindEnumElement, out int eventKindId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {

      int eventKindId = ExceptionAnalyzer.InferEventKindIdByException(ex);

#if !UsedByT4
      if (Routing.BizLoggerToTraceBus) {
        if (Routing.TraceBusExceptionsTextualizedToggle) {
          string renderedException = ExceptionRenderer.Render(ex);
          Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, eventKindId, renderedException);
        } else {
          Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex);
        }
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex);
      }
#else
      TraceBusFeed.Instance.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex); 
#endif
    }

    public static void Log(int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex) {

#if !UsedByT4
      if (Routing.BizLoggerToTraceBus) {
        if (Routing.TraceBusExceptionsTextualizedToggle) {
          string renderedException = ExceptionRenderer.Render(ex);
          Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, eventKindId, renderedException);
        } else {
          Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex);
        }
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex);
      }
#else
      TraceBusFeed.Instance.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex); 
#endif
    }

  }
}
