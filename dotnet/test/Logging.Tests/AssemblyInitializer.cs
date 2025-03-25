using Logging.SmartStandards;
using Logging.SmartStandards.Sinks;
using Logging.SmartStandards.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

      ExternalTraceBusListener = new TraceBusListener(PassTraceEventToTraceBusSink);

      Routing.UseCustomBus(PassLogEventToCustomBusSink);

      Routing.PassThruTraceBusToCustomBus = true;

      Routing.EnableEmittingToTraceBus(true);

      ExternalTraceBusFeed = new TraceBusFeed();

    }

    private static void PassTraceEventToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, object[] args
    ) {

      if ((args != null) && (args.Length > 0) && (args[0] is Exception)) {
        Exception ex = (Exception)args[0];
        // todo exceptions
      } else {
        TraceBusSink.WriteLogEvent(audience, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
      }

    }

    private static void PassLogEventToCustomBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate, object[] args
    ) {
      CustomBusSink.WriteLogEvent(audience, level, sourceContext, sourceLineId, eventId, messageTemplate, args);
    }

  }
}
