﻿using Logging.SmartStandards.TemplateHousekeeping;
using Logging.SmartStandards.Transport;
using System;
using System.Logging.SmartStandards.Internal;

namespace Logging.SmartStandards {

  public partial class BizLogger {

    public const string AudienceToken = "Biz";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";
      if (messageTemplate == null) messageTemplate = "";
      if (args == null) args = new object[0];

      if (Routing.BizLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
      }
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Enum logTemplate, params object[] args) {
      MessageTemplateRepository.GetMessageTemplateByEnum(logTemplate, out int eventId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, eventId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {

      int eventId = ExceptionAnalyzer.InferEventIdByException(ex);

      if (Routing.BizLoggerToTraceBus) {
        Routing.InternalTraceBusFeed.EmitException(AudienceToken, level, sourceContext, sourceLineId, eventId, ex);
      }

      if (Routing.BizLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(AudienceToken, level, sourceContext, sourceLineId, eventId, ex);
      }
    }
  }
}
