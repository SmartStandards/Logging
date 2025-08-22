using Logging.SmartStandards.EventKindManagement;
using Logging.SmartStandards.Internal;
using Logging.SmartStandards.Transport;
using System;

namespace Logging.SmartStandards {

  public partial class SecLogger {

    public const string AudienceToken = "Sec";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (messageTemplate == null) messageTemplate = "";

      if (args == null) args = new object[0];

#if !UsedByT4
      if (Routing.SecLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
      }

      if (Routing.SecLoggerToCustomBus) {
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
      if (Routing.SecLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex);
      }

      if (Routing.SecLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex);
      }
#else
      TraceBusFeed.Instance.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex); 
#endif
    }

    public static void Log(int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex) {

#if !UsedByT4
      if (Routing.SecLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex);
      }

      if (Routing.SecLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex);
      }
#else
      TraceBusFeed.Instance.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventKindId, ex); 
#endif
    }

  }
}
