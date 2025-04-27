using System;
using System.Diagnostics;
using Logging.SmartStandards.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards.ThirdPartyTests {

  [TestClass]
  public sealed class Simulate3rdPartyUsageTests {

    private int _NumberOfMessages;

    private int _NumberOfExceptions;

    [TestMethod]
    public void EmittingTraceEvents_TooEarly_ShouldBeBufferedAndFlushed() {

      Trace.WriteLine("Hä?");

      // SmartStandardsListener is not yet availabe, we emit anyways...

      int i;

      for (i = 0; i < 5; i++) {
        DevLogger.LogInformation("3rdPartySourceContext", 2072642824847130735, 42, "3rdParty Dev said: ", $"Hello {i}.");
        InsLogger.LogInformation("3rdPartySourceContext", 2073414608004986393, 43, "3rdParty Ins said: ", $"Hello {i}.");
        BizLogger.LogInformation("3rdPartySourceContext", 2073414611222994455, 44, "3rdParty Biz said: ", $"Hello {i}.");
      }

      // SmartStandardsListener will implicitely registered by the following line of code...

      //Routing.UseCustomBus(PassLogMessageToCustomBusSink, PassLogExceptionToTraceBusSink);

      // Emitting another event will flush the buffer (and deactivate it):

      TraceBusFeed.Instance.EmitMessage(
        "Dev", 2, "3rdPartySourceContext", 2072642824847130735, 42, "This one was emitted after SmartStandards initialized."
      );
    }

    private void PassLogMessageToCustomBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate, object[] args
    ) {
      _NumberOfMessages++;
    }

    private void PassLogExceptionToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int kindId, Exception ex
    ) {
      _NumberOfExceptions++;
    }

  }
}
