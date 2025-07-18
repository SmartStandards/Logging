﻿using Logging.SmartStandards;
using Logging.SmartStandards.Sinks;
using Logging.SmartStandards.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

[assembly: AssemblyMetadata("SourceContext", "SmartStandards-Logging")]

namespace Logging {

  [TestClass]
  public class AssemblyInitializer {

    public const string MySourceContextName = "SmartStandards-Logging";

    public static TraceBusListener ExternalTraceBusListener { get; set; }

    public static TraceBusFeed ExternalTraceBusFeed { get; set; }

    public static CheapInMemorySink TraceBusSink { get; set; } = new CheapInMemorySink();

    public static CheapInMemorySink CustomBusSink { get; set; } = new CheapInMemorySink();

    [AssemblyInitialize]
    public static void InitializeAssembly(TestContext testContext) {

      ExternalTraceBusListener = new TraceBusListener(PassTracedMessageToTraceBusSink, PassTracedExceptionToTraceBusSink);

      Routing.UseCustomBus(PassLogMessageToCustomBusSink, PassLogExceptionToTraceBusSink);

      ExternalTraceBusFeed = new TraceBusFeed();
    }

    private static void PassTracedMessageToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, object[] args
    ) {
      TraceBusSink.WriteMessage(audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
    }

    private static void PassTracedExceptionToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex
    ) {
      TraceBusSink.WriteException(audience, level, sourceContext, sourceLineId, eventKindId, ex);
    }

    private static void PassLogMessageToCustomBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, object[] args
    ) {
      CustomBusSink.WriteMessage(audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
    }

    private static void PassLogExceptionToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex
    ) {
      CustomBusSink.WriteException(audience, level, sourceContext, sourceLineId, eventKindId, ex);
    }

  }
}
