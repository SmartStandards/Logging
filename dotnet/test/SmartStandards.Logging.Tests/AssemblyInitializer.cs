using System;
using Logging.SmartStandards;
using Logging.SmartStandards.Sinks;
using Logging.SmartStandards.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.Tests {

  [TestClass]
  public class AssemblyInitializer {

    public static TraceBusListener ExternalTraceBusListener { get; set; }

    public static TraceBusFeed ExternalTraceBusFeed { get; set; }

    public static CheapInMemorySink TraceBusSink { get; set; } = new CheapInMemorySink();

    public static CheapInMemorySink CustomBusSink { get; set; } = new CheapInMemorySink();

    public const string SourceContext = "Logging.Tests";

    [AssemblyInitialize]
    public static void InitializeAssembly(TestContext testContext) {

      ExternalTraceBusListener = new TraceBusListener(PassTracedMessageToTraceBusSink, PassTracedExceptionToTraceBusSink);

      Routing.UseCustomBus(PassLogMessageToCustomBusSink, PassLogExceptionToTraceBusSink);

      Routing.EnableEmittingToTraceBus(true);

      ExternalTraceBusFeed = new TraceBusFeed();
    }

    private static void PassTracedMessageToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, object[] args
    ) {
      TraceBusSink.WriteMessage(audience, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
    }

    private static void PassTracedExceptionToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventId, Exception ex
    ) {
      TraceBusSink.WriteException(audience, level, sourceContext, sourceLineId, eventId, ex);
    }

    private static void PassLogMessageToCustomBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, object[] args
    ) {
      CustomBusSink.WriteMessage(audience, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
    }

    private static void PassLogExceptionToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventId, Exception ex
    ) {
      CustomBusSink.WriteException(audience, level, sourceContext, sourceLineId, eventId, ex);
    }

  }
}
