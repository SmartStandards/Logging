using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Logging.SmartStandards {

  [TestClass()]
  public class DevLoggerTests {

    [TestInitialize]
    public void InitializeBeforeEachTest() {

      // Make sure the lazy wire up doesn't happen before the LoggerTraceListener isn't initialized
      LoggerBase<DevLogger>.LogInformation(666, "Logging before initializing the LoggerTraceListener is not evil, but futile!");

      CheapInMemoryLogger.Start();
    }

    [TestCleanup]
    public void CleanupAfterEachTest() {
      CheapInMemoryLogger.Stop();
    }

    [TestMethod()]
    public void TraceMethods_WithNamedPlaceholders_ShouldEmitEscapedFormatString() {

      LoggerBase<DevLogger>.LogInformation(123, "Text without placeholders");

      LoggerBase<InfrastructureLogger>.LogWarning(222, "{thingy} space is low: {space} MB", "Disk", 5);

      LoggerBase<ProtocolLogger>.LogError(
        333,
        "User \"{UserLogonName}\" does not have sufficient rights to perform \"{Interaction}\" on environment \"{Environment}\".",
        new[] { "Müller", "Delete", "Productive" }
      );

      LoggerBase<DevLogger>.LogCritical(-12345, null, null);

      Assert.AreEqual(4, CheapInMemoryLogger.CollectedIds.Count);

      Assert.AreEqual("Dev", CheapInMemoryLogger.CollectedChannels[0]);
      Assert.AreEqual(2, CheapInMemoryLogger.CollectedLevels[0]);
      Assert.AreEqual(123, CheapInMemoryLogger.CollectedIds[0]);
      Assert.AreEqual("Text without placeholders", CheapInMemoryLogger.CollectedMessageTemplates[0]);
      Assert.IsNull(CheapInMemoryLogger.CollectedMessageArgs[0]);

      Assert.AreEqual("Ins", CheapInMemoryLogger.CollectedChannels[1]);
      Assert.AreEqual(3, CheapInMemoryLogger.CollectedLevels[1]);
      Assert.AreEqual(222, CheapInMemoryLogger.CollectedIds[1]);
      Assert.AreEqual("{thingy} space is low: {space} MB", CheapInMemoryLogger.CollectedMessageTemplates[1]);
      Assert.AreEqual("Disk", CheapInMemoryLogger.CollectedMessageArgs[1][0]);
      Assert.AreEqual("5", CheapInMemoryLogger.CollectedMessageArgs[1][1]);

      Assert.AreEqual("Pro", CheapInMemoryLogger.CollectedChannels[2]);
      Assert.AreEqual(4, CheapInMemoryLogger.CollectedLevels[2]);
      Assert.AreEqual(333, CheapInMemoryLogger.CollectedIds[2]);
      Assert.AreEqual("User \"{UserLogonName}\" does not have sufficient rights to perform \"{Interaction}\" on environment \"{Environment}\".", CheapInMemoryLogger.CollectedMessageTemplates[2]);
      Assert.AreEqual("Müller", CheapInMemoryLogger.CollectedMessageArgs[2][0]);
      Assert.AreEqual("Delete", CheapInMemoryLogger.CollectedMessageArgs[2][1]);
      Assert.AreEqual("Productive", CheapInMemoryLogger.CollectedMessageArgs[2][2]);

      Assert.AreEqual("Dev", CheapInMemoryLogger.CollectedChannels[3]);
      Assert.AreEqual(5, CheapInMemoryLogger.CollectedLevels[3]); // Critical
      Assert.AreEqual(12345, CheapInMemoryLogger.CollectedIds[3]);
      Assert.IsNull(CheapInMemoryLogger.CollectedMessageTemplates[3]);
      Assert.IsNull(CheapInMemoryLogger.CollectedMessageArgs[3]);

    }

    [TestMethod()]
    public void TryTrace_BorderCases_DontThrowExceptions() {

      LoggerBase<DevLogger>.LogReturnCodeAsError(123, "Simple with {placeholder}.", new[] { "resolved placeholder" });

      LoggerBase<DevLogger>.LogReturnCodeAsError(-456, "Error with args = null.", null);

      LoggerBase<DevLogger>.LogReturnCodeAsWarning(-789, "Warning with empty placeholder array.", Array.Empty<object>());

      Assert.AreEqual(3, CheapInMemoryLogger.CollectedIds.Count);

      Assert.AreEqual("Dev", CheapInMemoryLogger.CollectedChannels[0]);
      Assert.AreEqual(2, CheapInMemoryLogger.CollectedLevels[0]); // Info, because returnCode was positive
      Assert.AreEqual(123, CheapInMemoryLogger.CollectedIds[0]);
      Assert.AreEqual("Simple with {placeholder}.", CheapInMemoryLogger.CollectedMessageTemplates[0]);
      Assert.AreEqual("resolved placeholder", CheapInMemoryLogger.CollectedMessageArgs[0][0]);

      Assert.AreEqual("Dev", CheapInMemoryLogger.CollectedChannels[1]);
      Assert.AreEqual(4, CheapInMemoryLogger.CollectedLevels[1]); // Error, because returnCode was negative
      Assert.AreEqual(456, CheapInMemoryLogger.CollectedIds[1]); // ID must (always) be positive
      Assert.AreEqual("Error with args = null.", CheapInMemoryLogger.CollectedMessageTemplates[1]);
      Assert.IsNull(CheapInMemoryLogger.CollectedMessageArgs[1]);

      Assert.AreEqual("Dev", CheapInMemoryLogger.CollectedChannels[2]);
      Assert.AreEqual(3, CheapInMemoryLogger.CollectedLevels[2]); // Warning, because returnCode was negative
      Assert.AreEqual(789, CheapInMemoryLogger.CollectedIds[2]); // ID must (always) be positive
      Assert.AreEqual("Warning with empty placeholder array.", CheapInMemoryLogger.CollectedMessageTemplates[2]);
      Assert.IsNull(CheapInMemoryLogger.CollectedMessageArgs[2]);

    }

  }
}
