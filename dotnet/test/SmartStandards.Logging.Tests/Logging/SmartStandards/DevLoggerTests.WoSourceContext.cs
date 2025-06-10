using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  public partial class DevLoggerTests {


    [TestMethod()]
    public void LogMethods_WoSourceContext_ShouldDefaultToCurrentAssemblyName() {

      DevLogger.LogInformation(2072416880134254029, 7, "{thingy} space is low: {space} MB", "Disk", 5);

      Assert.AreEqual(AssemblyInitializer.MySourceContextName, AssemblyInitializer.CustomBusSink.CollectedSourceContexts[0]);

      MyAssert.CustomBusSinkContains(
        0, "Dev", 2, AssemblyInitializer.MySourceContextName, 2072416880134254029, 7, "{thingy} space is low: {space} MB"
      );

      Assert.AreEqual("Disk", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[0][0]);
      Assert.AreEqual(5, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[0][1]);

    }

  }
}
