using System;
using Logging.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  [TestClass]
  public class DevLoggerTests {

    [TestInitialize]
    public void InitializeBeforeEachTest() {

      CheapInMemorySink.Clear();
      MockingTraceListener.OnLogEventReceived = CheapInMemorySink.WriteLogEntry;
    }

    [TestCleanup]
    public void CleanupAfterEachTest() {
      MockingTraceListener.OnLogEventReceived = null;
      CheapInMemorySink.Clear();
    }

    [TestMethod()]
    public void TraceMethods_WithNamedPlaceholders_ShouldEmitEscapedFormatString() {

      Assert.AreEqual(0, CheapInMemorySink.CollectedEventIds.Count);

      DevLogger.LogInformation(AssemblyInitializer.SourceContext, 1, "Text without placeholders, zero args provided"); // Expected: MessageArgs == null

      Assert.AreEqual(AssemblyInitializer.SourceContext, CheapInMemorySink.CollectedSourceContexts[0]);
      //Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[0]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[0]);
      Assert.AreEqual(1, CheapInMemorySink.CollectedEventIds[0]);
      Assert.AreEqual("(DEV) Text without placeholders, zero args provided", CheapInMemorySink.CollectedMessageTemplates[0]);
      //HACK: das hat den zus. Overload abgesichert, den es nur für Information gab, und der weggelöscht werden sollte!
      //Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[0]);

      DevLogger.LogInformation(AssemblyInitializer.SourceContext, 2, "Text without placeholders, args = null.", null); // Expected: MessageArgs == null

      //Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[1]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[1]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedEventIds[1]);
      Assert.AreEqual("(DEV) Text without placeholders, args = null.", CheapInMemorySink.CollectedMessageTemplates[1]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[1]);

      DevLogger.LogInformation(AssemblyInitializer.SourceContext, 3, "Text without placeholders, args = {null}.", new object[] { null }); // Expected: MessageArgs == { null }

      //Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[2]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[2]);
      Assert.AreEqual(3, CheapInMemorySink.CollectedEventIds[2]);
      Assert.AreEqual("(DEV) Text without placeholders, args = {null}.", CheapInMemorySink.CollectedMessageTemplates[2]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[2][0]);

      object[] nullArray = null;
      DevLogger.LogInformation(AssemblyInitializer.SourceContext, 4, "Text without placeholders, args = nullArray.", nullArray);// Expected: MessageArgs == null

      //Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[3]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[3]);
      Assert.AreEqual(4, CheapInMemorySink.CollectedEventIds[3]);
      Assert.AreEqual("(DEV) Text without placeholders, args = nullArray.", CheapInMemorySink.CollectedMessageTemplates[3]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[3]);

      string nullString = null;
      DevLogger.LogInformation(AssemblyInitializer.SourceContext, 5, "Text without placeholders, args = nullString.", nullString); // Expected: MessageArgs == { null }

      //Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[4]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[4]);
      Assert.AreEqual(5, CheapInMemorySink.CollectedEventIds[4]);
      Assert.AreEqual("(DEV) Text without placeholders, args = nullString.", CheapInMemorySink.CollectedMessageTemplates[4]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[4][0]);

      DevLogger.LogInformation(AssemblyInitializer.SourceContext, 6, "Text without placeholders, args = null, null.", null, null); // Expected: MessageArgs == { null, null }

      //Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[5]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[5]);
      Assert.AreEqual(6, CheapInMemorySink.CollectedEventIds[5]);
      Assert.AreEqual("(DEV) Text without placeholders, args = null, null.", CheapInMemorySink.CollectedMessageTemplates[5]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[5][0]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[5][1]);

      InsLogger.LogWarning(AssemblyInitializer.SourceContext, 7, "{thingy} space is low: {space} MB", "Disk", 5);

      //Assert.AreEqual("Ins", CheapInMemorySink.CollectedAudiences[6]);
      Assert.AreEqual(3, CheapInMemorySink.CollectedLevels[6]);
      Assert.AreEqual(7, CheapInMemorySink.CollectedEventIds[6]);
      Assert.AreEqual("(INS) {thingy} space is low: {space} MB", CheapInMemorySink.CollectedMessageTemplates[6]);
      Assert.AreEqual("Disk", CheapInMemorySink.CollectedMessageArgs[6][0]);
      Assert.AreEqual(5, CheapInMemorySink.CollectedMessageArgs[6][1]);

      BizLogger.LogError(
        AssemblyInitializer.SourceContext,
        8,
        "User \"{UserLogonName}\" does not have sufficient rights to perform \"{Interaction}\" on environment \"{Environment}\".",
        "Müller", "Delete", "Productive"
      );

      //Assert.AreEqual("Pro", CheapInMemorySink.CollectedAudiences[7]);
      Assert.AreEqual(4, CheapInMemorySink.CollectedLevels[7]);
      Assert.AreEqual(8, CheapInMemorySink.CollectedEventIds[7]);
      Assert.AreEqual("(BIZ) User \"{UserLogonName}\" does not have sufficient rights to perform \"{Interaction}\" on environment \"{Environment}\".", CheapInMemorySink.CollectedMessageTemplates[7]);
      Assert.AreEqual("Müller", CheapInMemorySink.CollectedMessageArgs[7][0]);
      Assert.AreEqual("Delete", CheapInMemorySink.CollectedMessageArgs[7][1]);
      Assert.AreEqual("Productive", CheapInMemorySink.CollectedMessageArgs[7][2]);

      DevLogger.LogCritical(AssemblyInitializer.SourceContext, -12345, null, null);

      //Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[8]);
      Assert.AreEqual(5, CheapInMemorySink.CollectedLevels[8]); // Critical
      Assert.AreEqual(-12345, CheapInMemorySink.CollectedEventIds[8]);
      Assert.AreEqual("(DEV) ", CheapInMemorySink.CollectedMessageTemplates[8]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[8]);

      Assert.AreEqual(9, CheapInMemorySink.CollectedEventIds.Count);
    }

    [TestMethod()]
    public void LogReturnCodeAs_Codes_TranslateCorrectlyToLevels() {

      DevLogger.LogReturnCodeAsError(AssemblyInitializer.SourceContext, 123, "Simple with {placeholder}.", new[] { "resolved placeholder" });

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[0]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[0]); // Info, because returnCode was positive
      Assert.AreEqual(123, CheapInMemorySink.CollectedEventIds[0]);
      Assert.AreEqual("Simple with {placeholder}.", CheapInMemorySink.CollectedMessageTemplates[0]);
      Assert.AreEqual("resolved placeholder", CheapInMemorySink.CollectedMessageArgs[0][0]);

      DevLogger.LogReturnCodeAsError(AssemblyInitializer.SourceContext, -456, "Error with args = null.", null);

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[1]);
      Assert.AreEqual(4, CheapInMemorySink.CollectedLevels[1]); // Error, because returnCode was negative
      Assert.AreEqual(456, CheapInMemorySink.CollectedEventIds[1]); // ID must (always) be positive
      Assert.AreEqual("Error with args = null.", CheapInMemorySink.CollectedMessageTemplates[1]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[1]);

      DevLogger.LogReturnCodeAsWarning(AssemblyInitializer.SourceContext, -789, "Warning with empty placeholder array.", Array.Empty<object>());

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedAudiences[2]);
      Assert.AreEqual(3, CheapInMemorySink.CollectedLevels[2]); // Warning, because returnCode was negative
      Assert.AreEqual(789, CheapInMemorySink.CollectedEventIds[2]); // ID must (always) be positive
      Assert.AreEqual("Warning with empty placeholder array.", CheapInMemorySink.CollectedMessageTemplates[2]);
      Assert.AreEqual(0, CheapInMemorySink.CollectedMessageArgs[2].Length);

      Assert.AreEqual(3, CheapInMemorySink.CollectedEventIds.Count);
    }

  }
}
