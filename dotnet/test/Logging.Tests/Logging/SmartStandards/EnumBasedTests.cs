using System.Globalization;
using System.Threading;
using Logging.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  [TestClass()]
  public class EnumBasedTests {

    [TestMethod()]
    public void EnumBasedLoggingTest1() {

      string capturedMessageFromLogMethod = null;

      CultureInfo preservedCulture = Thread.CurrentThread.CurrentCulture;
      try {

        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        DevLogger.LogWarning(AssemblyInitializer.SourceContext, WellknownMockMessages.ZuVielFooImBar);
        Assert.AreEqual("There is too much foo within bar beacause of {0}!", capturedMessageFromLogMethod); //< pass-trough

        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
        DevLogger.LogWarning(AssemblyInitializer.SourceContext, WellknownMockMessages.ZuVielFooImBar);
        Assert.AreEqual("Da ist zu viel Foo im Bar wegen {0}!", capturedMessageFromLogMethod); //< pass-trough

      } finally {
        Thread.CurrentThread.CurrentCulture = preservedCulture;
      }

    }

  }

}
