using Logging.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  public partial class DevLoggerTests {

    [TestMethod()]
    public void LogMethods_WoSourceContext_ShouldDefaultToCurrentAssemblyName() {

      string currentAssemblyName = "SmartStandards.Logging.net8.0";

      DevLogger.LogInformation(2072416880134254029, 7, "{thingy} space is low: {space} MB", "Disk", 5);

      MyAssert.CustomBusSinkContains(
        0, "Dev", 2, currentAssemblyName, 2072416880134254029, 7, "{thingy} space is low: {space} MB"
      );
      Assert.AreEqual("Disk", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[0][0]);
      Assert.AreEqual(5, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[0][1]);

    }

  }
}
