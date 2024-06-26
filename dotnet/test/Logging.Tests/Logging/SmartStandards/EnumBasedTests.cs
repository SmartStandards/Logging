using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Discovery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading;

namespace Logging.SmartStandards {

  [TestClass()]
  public class EnumBasedTests {

    [TestMethod()] 
    public void EnumBasedLoggingTest1() {

        string capturedMessageFromLogMethod = null;
        DevLogger.ConfigureRedirection(
          (string c, int l, int id, string msg, object[] args) => {
            capturedMessageFromLogMethod = msg;
          },
          logExceptionMethod: null,
          forwardDirectInputToTracing: false,
          forwardTracingInputToLogMehod: false
        );

        CultureInfo preservedCulture = Thread.CurrentThread.CurrentCulture;
        try {

          Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
          DevLogger.LogWarning(WellknownMockMessages.ZuVielFooImBar);
          Assert.AreEqual("There is too much foo within bar beacause of {0}!", capturedMessageFromLogMethod); //< pass-trough

          Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
          DevLogger.LogWarning(WellknownMockMessages.ZuVielFooImBar);
          Assert.AreEqual("Da ist zu viel Foo im Bar wegen {0}!", capturedMessageFromLogMethod); //< pass-trough

        }
        finally {
          Thread.CurrentThread.CurrentCulture = preservedCulture;
        }

    }

  }

}
