using Logging.SmartStandards.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Logging.SmartStandards.ThirdPartyTests {

  [TestClass]
  public sealed class Simulate3rdPartyUsageTests {

    private int _NumberOfMessages;

    private int _NumberOfExceptions;

    [TestMethod]
    public void EmittingTraceEvents_TooEarly_ShouldBeBufferedAndFlushed() {

      Trace.WriteLine("How can this WriteLine() reach VS Output without anything in Trace.Listeners?");
      Trace.TraceInformation("How can this TraceInformation() reach VS Output without anything in Trace.Listeners?");

      // SmartStandardsListener is not yet availabe, we emit anyways...

      int i;

      for (i = 0; i < 5; i++) {
        DevLogger.LogInformation("3rdPartySourceContext", 2072642824847130735, 42, "3rdParty Dev said: ", $"Hello {i}.");
        InsLogger.LogInformation("3rdPartySourceContext", 2073414608004986393, 43, "3rdParty Ins said: ", $"Hello {i}.");
        BizLogger.LogInformation("3rdPartySourceContext", 2073414611222994455, 44, "3rdParty Biz said: ", $"Hello {i}.");
        SecLogger.LogInformation("3rdPartySourceContext", 2078719086756592152, 45, "3rdParty Sec said: ", $"Hello {i}.");
      }

      // SmartStandardsListener will implicitely registered by the following line of code...

      //Routing.UseCustomBus(PassLogMessageToCustomBusSink, PassLogExceptionToTraceBusSink);

      // Emitting another event will flush the buffer (and deactivate it):

      TraceBusFeed.Instance.EmitMessage(
        "Dev", 2, "3rdPartySourceContext", 2072642824847130735, 42, "This one was emitted after SmartStandards initialized."
      );
    }

    private void PassLogMessageToCustomBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate, object[] args
    ) {
      _NumberOfMessages++;
    }

    private void PassLogExceptionToTraceBusSink(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, Exception ex
    ) {
      _NumberOfExceptions++;
    }

  }
}
