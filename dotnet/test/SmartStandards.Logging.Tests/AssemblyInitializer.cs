using Logging.SmartStandards;
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

    public static TraceBusListener ReadyToReadTraceBusListener { get; set; }

    public static TraceBusListener RawTraceBusListener { get; set; }

    public static TraceBusFeed ExternalTraceBusFeed { get; set; }

    public static CheapInMemorySink ReadyToReadTraceBusSink { get; set; } = new CheapInMemorySink();

    public static CheapInMemorySink RawTraceBusSink { get; set; } = new CheapInMemorySink();

    public static CheapInMemorySink CustomBusSink { get; set; } = new CheapInMemorySink();

    [AssemblyInitialize]
    public static void InitializeAssembly(TestContext testContext) {

      ReadyToReadTraceBusListener = new TraceBusListener(PassMessageToReadyToReadTraceBusSink, PassExceptionToReadyToReadTraceBusSink) {
        Name = "ReadyToReadTraceBusListener"
      };
      RawTraceBusListener = new TraceBusListener(PassMessageToRawTraceBusSink, PassExceptionToRawTraceBusSink) {
        Name = "RawTraceBusListener"
      };
      // ^ our implementation of TraceBusListener is self-registering to the global Trace.Listeners collection.

      Routing.TraceBusRawMode.Add("RawTraceBusListener");

      Routing.UseCustomBus(PassMessageToCustomBusSink, PassExceptionToCustomBusSink);

      ExternalTraceBusFeed = new TraceBusFeed();
      ExternalTraceBusFeed.RawModeListeners.Add("RawTraceBusListener");
    }

    private static void PassMessageToReadyToReadTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, object[] args
    ) {
      ReadyToReadTraceBusSink.WriteMessage(audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
    }

    private static void PassExceptionToReadyToReadTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex
    ) {
      ReadyToReadTraceBusSink.WriteException(audience, level, sourceContext, sourceLineId, eventKindId, ex);
    }

    private static void PassMessageToRawTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, object[] args
    ) {
      RawTraceBusSink.WriteMessage(audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
    }

    private static void PassExceptionToRawTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex
    ) {
      RawTraceBusSink.WriteException(audience, level, sourceContext, sourceLineId, eventKindId, ex);
    }

    private static void PassMessageToCustomBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, object[] args
    ) {
      CustomBusSink.WriteMessage(audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args);
    }

    private static void PassExceptionToCustomBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex
    ) {
      CustomBusSink.WriteException(audience, level, sourceContext, sourceLineId, eventKindId, ex);
    }

  }
}
