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

      // Return code 0 => Log level 0 "Trace"

      ReturnCodeLogger.DevLog(2078308604637680152L, "FooMethod", 0, null, null);

      MyAssert.BothSinksContain(
        i, "Dev", 0, "SmartStandards-Logging", 2078308604637680152L, // 0 = Trace level
        0, "ReturnCode {ReturnCodeToBeLogged} from {CalledMethodToBeLogged}: "
      );

      // Return code -1 => Log level 0 "Trace"

      i++;

      ReturnCodeLogger.InsLog(2078309453599183369L, "FooMethod", -1, "StatusMessageTemplate and {FooValue} of FooMethod.", new object[] { 4711 });

      MyAssert.BothSinksContain(
        i, "Ins", 0, "SmartStandards-Logging", 2078309453599183369L, // 0 = Trace level
        0, "ReturnCode {ReturnCodeToBeLogged} from {CalledMethodToBeLogged}: StatusMessageTemplate and {FooValue} of FooMethod."
      );

      Assert.AreEqual(-1, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][0]);
      Assert.AreEqual("FooMethod", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][1]);
      Assert.AreEqual(4711, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][2]);

      // Return code -300000000 => Log level 4 "error"

      i++;

      ReturnCodeLogger.BizLog("FooSourceContext", 2078309617814558529L, "FooMethod", -300000000, "StatusMessageTemplate and {FooValue} of FooMethod.", new object[] { 4711 });

      MyAssert.BothSinksContain(
        i, "Biz", 4, "FooSourceContext", 2078309617814558529L,
        0, "ReturnCode {ReturnCodeToBeLogged} from {CalledMethodToBeLogged}: StatusMessageTemplate and {FooValue} of FooMethod."
      );

      // Return code -400000000 => Log level 5 "critical"

      i++;

      ReturnCodeLogger.DevLog("FooSourceContext", 2078532114646466475L, "FooMethod", -400000000, "Invalid Argument {FooArgument} of FooMethod.", new object[] { "broken" });

      MyAssert.BothSinksContain(
       i, "Dev", 5, "FooSourceContext", 2078532114646466475L,
       0, "ReturnCode {ReturnCodeToBeLogged} from {CalledMethodToBeLogged}: Invalid Argument {FooArgument} of FooMethod."
     );

    }

    [TestMethod()]
    public void Textualize_BasicCases_CreateExpectedStrings() {

      string actual = ReturnCodeLogger.Textualize(
        "FooMethod", -1, "Called Method's StatusMessageTemplate with a {Value} placeholder.", new object[] { "resolved" }
      );

      Assert.AreEqual(
        "ReturnCode -1 from FooMethod: Called Method's StatusMessageTemplate with a resolved placeholder.",
        actual
      );

    }
  }
}
