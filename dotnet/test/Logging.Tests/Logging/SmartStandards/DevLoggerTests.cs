﻿using Logging.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Threading;

namespace Logging.SmartStandards {

  [TestClass]
  public class DevLoggerTests {

    private string Sc { get { return AssemblyInitializer.SourceContext; } }

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

      //

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
    public void LogMethods_EnumSupport_ShouldResolveCorrectly() {

      int i = 0;

      CultureInfo preservedCulture = Thread.CurrentThread.CurrentCulture;
      try {

        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        DevLogger.LogWarning(AssemblyInitializer.SourceContext, 2071873991520386244, WellknownMockMessages.ZuVielFooImBar);

        MyAssert.BothSinksContain(
          i, "Dev", 3, Sc, 2071873991520386244, (int)WellknownMockMessages.ZuVielFooImBar,
          "There is too much foo within bar beacause of {0}!"
        );

        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

        i++;
        DevLogger.LogWarning(AssemblyInitializer.SourceContext, 2071873995903461991, WellknownMockMessages.ZuVielFooImBar);

        MyAssert.BothSinksContain(
          i, "Dev", 3, Sc, 2071873995903461991, (int)WellknownMockMessages.ZuVielFooImBar,
          "Da ist zu viel Foo im Bar wegen {0}!"
        );

      } finally {
        Thread.CurrentThread.CurrentCulture = preservedCulture;
      }

    }

    //[TestMethod()]
    //public void LogReturnCodeAs_Codes_TranslateCorrectlyToLevels() {

    //  DevLogger.LogReturnCodeAsError(Sc, 2071873958028081940, 123, "Simple with {placeholder}.", new[] { "resolved placeholder" });

    //  Assert.AreEqual("Dev", AssemblyInitializer.CustomBusSink.CollectedAudiences[0]);
    //  Assert.AreEqual(2, AssemblyInitializer.CustomBusSink.CollectedLevels[0]); // Info, because returnCode was positive
    //  Assert.AreEqual(123, AssemblyInitializer.CustomBusSink.CollectedEventIds[0]);
    //  Assert.AreEqual("Simple with {placeholder}.", AssemblyInitializer.CustomBusSink.CollectedMessageTemplates[0]);
    //  Assert.AreEqual("resolved placeholder", AssemblyInitializer.CustomBusSink.CollectedMessageArgs[0][0]);

    //  DevLogger.LogReturnCodeAsError(Sc, -456, "Error with args = null.", null);

    //  Assert.AreEqual("Dev", AssemblyInitializer.CustomBusSink.CollectedAudiences[1]);
    //  Assert.AreEqual(4, AssemblyInitializer.CustomBusSink.CollectedLevels[1]); // Error, because returnCode was negative
    //  Assert.AreEqual(456, AssemblyInitializer.CustomBusSink.CollectedEventIds[1]); // ID must (always) be positive
    //  Assert.AreEqual("Error with args = null.", AssemblyInitializer.CustomBusSink.CollectedMessageTemplates[1]);
    //  Assert.IsNull(AssemblyInitializer.CustomBusSink.CollectedMessageArgs[1]);

    //  DevLogger.LogReturnCodeAsWarning(Sc, -789, "Warning with empty placeholder array.", Array.Empty<object>());

    //  Assert.AreEqual("Dev", AssemblyInitializer.CustomBusSink.CollectedAudiences[2]);
    //  Assert.AreEqual(3, AssemblyInitializer.CustomBusSink.CollectedLevels[2]); // Warning, because returnCode was negative
    //  Assert.AreEqual(789, AssemblyInitializer.CustomBusSink.CollectedEventIds[2]); // ID must (always) be positive
    //  Assert.AreEqual("Warning with empty placeholder array.", AssemblyInitializer.CustomBusSink.CollectedMessageTemplates[2]);
    //  Assert.AreEqual(0, AssemblyInitializer.CustomBusSink.CollectedMessageArgs[2].Length);

    //  Assert.AreEqual(3, AssemblyInitializer.CustomBusSink.CollectedEventIds.Count);
    //}

  }
}
