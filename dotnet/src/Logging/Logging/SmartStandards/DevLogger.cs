﻿using System;
using System.Logging.SmartStandards;

namespace Logging.SmartStandards {

  public partial class DevLogger {

    public const string AudienceToken = "Dev";

    public const string AudienceTokenU = "DEV";

    public static void Log(string sourceContext, int level, int id, string messageTemplate, params object[] args) {

      if (Routing.DevLoggerToTraceBus) {
        TraceBusFeed.EmitMessage(sourceContext, AudienceTokenU, level, id, messageTemplate, args);
      }

      if (Routing.DevLoggerToCustomBus) {
        CustomBusFeed.OnEmitMessage.Invoke(sourceContext, AudienceToken, level, id, messageTemplate, args);
      }
    }

    public static void Log(string sourceContext, int level, Enum logTemplate, params object[] args) {
      LoggingHelper.GetLogTemplateFromEnum(logTemplate, out int id, out string messageTemplate);
      Log(sourceContext, level, id, messageTemplate, args);
    }

    public static void Log(string sourceContext, int level, Exception ex) {

      if (Routing.DevLoggerToTraceBus) {
        TraceBusFeed.EmitException(sourceContext, AudienceTokenU, level, ex);
      }

      if (Routing.DevLoggerToCustomBus) {
        CustomBusFeed.OnEmitException.Invoke(sourceContext, AudienceToken, level, ex);
      }
    }
  }
}