using Logging.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading;

namespace Logging.SmartStandards {

  [TestClass]
  public partial class DevLoggerTests {

    private static string Sc { get { return AssemblyInitializer.SourceContext; } }

    [TestInitialize]
    public void InitializeBeforeEachTest() {
      AssemblyInitializer.TraceBusSink.Clear();
      AssemblyInitializer.CustomBusSink.Clear();
    }

    [TestCleanup]
    public void CleanupAfterEachTest() {
      AssemblyInitializer.TraceBusSink.Clear();
      AssemblyInitializer.CustomBusSink.Clear();
    }

    [TestMethod()]
    public void LogMethods_BasicCases_ShouldArriveInCustomSinkOnceAndInTraceBusSink() {

      int i = 0;

      DevLogger.LogInformation(Sc, 2071873252511884979, 1, "Text without placeholders, zero args provided");

      MyAssert.BothSinksContain(i, "Dev", 2, Sc, 2071873252511884979, 1, "Text without placeholders, zero args provided");
      Assert.AreEqual(0, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i].Length);

      // null as args => should be normalized to empty strings and empty array

      i++;
      DevLogger.LogInformation(null, 2071873589126733913, 2, null, null);

      MyAssert.BothSinksContain(i, "Dev", 2, "UnknownSourceContext", 2071873589126733913, 2, "");
      Assert.AreEqual(0, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i].Length);

      //

      i++;
      InsLogger.LogWarning(Sc, 2071873946794501890, 7, "{thingy} space is low: {space} MB", "Disk", 5);

      MyAssert.BothSinksContain(i, "Ins", 3, Sc, 2071873946794501890, 7, "{thingy} space is low: {space} MB");
      Assert.AreEqual("Disk", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][0]);
      Assert.AreEqual(5, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][1]);

      //

      i++;
      BizLogger.LogError(
        Sc, 2071873950133171447, 8,
        "User \"{UserLogonName}\" does not have sufficient rights to perform \"{Interaction}\" on environment \"{Environment}\".",
        "Müller", "Delete", "Productive"
      );

      MyAssert.BothSinksContain(
        i, "Biz", 4, Sc, 2071873950133171447, 8,
        "User \"{UserLogonName}\" does not have sufficient rights to perform \"{Interaction}\" on environment \"{Environment}\"."
      );
      Assert.AreEqual("Müller", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][0]);
      Assert.AreEqual("Delete", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][1]);
      Assert.AreEqual("Productive", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][2]);

      //

      i++;
      DevLogger.LogCritical(Sc, 2071876994403864019, -12345, null, null);

      MyAssert.BothSinksContain(i, "Dev", 5, Sc, 2071876994403864019, -12345, "");

      // Exception

      Exception ex = new Exception("MockException");

      i++;
      DevLogger.LogCritical(Sc, 2071926793372485828, ex);

      MyAssert.TraceBusSinkContains(i, "Dev", 5, Sc, 2071926793372485828, 1969630032, "MockException\r\n-- System.Exception --", null);
      MyAssert.CustomBusSinkContains(i, "Dev", 5, Sc, 2071926793372485828, 1969630032, null, ex);

      // Exception wrapped

      Exception ex2 = ex.Wrap(1234, "Zwiebel.");

      i++;
      DevLogger.LogCritical(Sc, 2071926793372485829, ex2);

      MyAssert.TraceBusSinkContains(i, "Dev", 5, Sc, 2071926793372485829, 1234, "Zwiebel. #1234 :: MockException\r\n-- Logging.SmartStandards.ExceptionExtensions+WrappedException --\r\n-- System.Exception -- (inner)", null);
      MyAssert.CustomBusSinkContains(i, "Dev", 5, Sc, 2071926793372485829, 1234, null, ex2);

      // Ensure PassThruTraceBusToCustomBus is working:

      i++;
      AssemblyInitializer.ExternalTraceBusFeed.EmitMessage("Dev", 2, Sc, 2071880606768384068, 4711, "Das kam direkt vom TraceBus", 123, "Foo");

      MyAssert.BothSinksContain(
        i, "Dev", 2, Sc, 2071880606768384068, 4711,
        "Das kam direkt vom TraceBus"
      );
      Assert.AreEqual(123, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][0]);
      Assert.AreEqual("Foo", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[i][1]);

      //

      Assert.AreEqual(i, AssemblyInitializer.TraceBusSink.CollectedEventIds.Count - 1);
      Assert.AreEqual(i, AssemblyInitializer.CustomBusSink.CollectedEventIds.Count - 1);
    }

    [TestMethod()]
    public void LogMethods_KindFromEnum_ShouldResolveCorrectly() {

      int i = 0;

      CultureInfo preservedCulture = Thread.CurrentThread.CurrentCulture;
      try {

        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        DevLogger.LogWarning(AssemblyInitializer.SourceContext, 2071873991520386244, TestingLogEventKind.ZuVielFooImBar);

        MyAssert.BothSinksContain(
          i, "Dev", 3, Sc, 2071873991520386244, (int)TestingLogEventKind.ZuVielFooImBar,
          "There is too much foo within bar beacause of {0}!"
        );

        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

        i++;
        DevLogger.LogWarning(AssemblyInitializer.SourceContext, 2071873995903461991, TestingLogEventKind.ZuVielFooImBar);

        MyAssert.BothSinksContain(
          i, "Dev", 3, Sc, 2071873995903461991, (int)TestingLogEventKind.ZuVielFooImBar,
          "Da ist zu viel Foo im Bar wegen {0}!"
        );

      } finally {
        Thread.CurrentThread.CurrentCulture = preservedCulture;
      }

    }
  }
}
