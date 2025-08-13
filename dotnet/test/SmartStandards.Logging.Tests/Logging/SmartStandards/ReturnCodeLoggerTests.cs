using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  [TestClass]
  public class ReturnCodeLoggerTests {

    [TestInitialize]
    public void InitializeBeforeEachTest() {
      AssemblyInitializer.ReadyToReadTraceBusSink.Clear();
      AssemblyInitializer.RawTraceBusSink.Clear();
      AssemblyInitializer.CustomBusSink.Clear();
    }

    [TestCleanup]
    public void CleanupAfterEachTest() {
      AssemblyInitializer.ReadyToReadTraceBusSink.Clear();
      AssemblyInitializer.RawTraceBusSink.Clear();
      AssemblyInitializer.CustomBusSink.Clear();
    }

    [TestMethod()]
    public void LogMethods_BasicCases_LoggedAsExpected() {

      int i = 0;

      ReturnCodeLogger.DevLog(2078308604637680152L, "FooMethod", 0, null, null);

      MyAssert.BothSinksContain(
        i, "Dev", 2, "SmartStandards-Logging", 2078308604637680152L,
        0, "ReturnCode {ReturnCodeToBeLogged} from {CalledMethodToBeLogged}: "
      );

      i++;

      ReturnCodeLogger.InsLog(2078309453599183369L, "FooMethod", -1, "StatusMessageTemplate and {FooValue} of FooMethod.", new object[] { 4711 });

      MyAssert.BothSinksContain(
        i, "Ins", 4, "SmartStandards-Logging", 2078309453599183369L,
        0, "ReturnCode {ReturnCodeToBeLogged} from {CalledMethodToBeLogged}: StatusMessageTemplate and {FooValue} of FooMethod."
      );

      Assert.AreEqual(-1, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][0]);
      Assert.AreEqual("FooMethod", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][1]);
      Assert.AreEqual(4711, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][2]);

      i++;

      ReturnCodeLogger.BizLog("FooSourceContext", 2078309617814558529L, "FooMethod", -1, "StatusMessageTemplate and {FooValue} of FooMethod.", new object[] { 4711 });

      MyAssert.BothSinksContain(
        i, "Biz", 4, "FooSourceContext", 2078309617814558529L,
        0, "ReturnCode {ReturnCodeToBeLogged} from {CalledMethodToBeLogged}: StatusMessageTemplate and {FooValue} of FooMethod."
      );

    }

  }
}
