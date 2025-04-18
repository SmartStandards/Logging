﻿using Logging.SmartStandards.Transport;
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

      // SmartStandardsListener is not yet availabe, we emit anyways...

      int i;

      for (i = 0; i < 5; i++) {
        TraceBusFeed.Instance.EmitMessage("Dev", 2, "3rdPartySourceContext", 2072642824847130735, 42, "3rdParty said: ", $"Hello {i}.");
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
