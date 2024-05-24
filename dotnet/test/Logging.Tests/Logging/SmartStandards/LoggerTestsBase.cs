using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Logging.SmartStandards {

  public abstract class LoggerTestsBase {

    internal abstract bool UseTrace { get; }

    private string ExpectedPipeKind {
      get {
        if (this.UseTrace) {
          return "trace";
        } else {
          return "direct";
        }
      }
    }

    [TestInitialize]
    public void InitializeBeforeEachTest() {

      // Make sure the lazy wire up doesn't happen before the LoggerTraceListener isn't initialized
      LoggerBase<DevLogger>.LogInformation(666, "Logging before initializing the LoggerTraceListener is not evil, but futile!");

      CheapInMemorySink.UseTrace = this.UseTrace;

      CheapInMemorySink.Start();
    }

    [TestCleanup]
    public void CleanupAfterEachTest() {
      CheapInMemorySink.Stop();
    }

    [TestMethod()]
    public void TraceMethods_WithNamedPlaceholders_ShouldEmitEscapedFormatString() {

      Assert.AreEqual(0, CheapInMemorySink.CollectedIds.Count);

      DevLogger.LogInformation(1, "Text without placeholders, zero args provided"); // Expected: MessageArgs == null

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[0]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[0]);
      Assert.AreEqual(1, CheapInMemorySink.CollectedIds[0]);
      Assert.AreEqual("Text without placeholders, zero args provided", CheapInMemorySink.CollectedMessageTemplates[0]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[0]);

      DevLogger.LogInformation(2, "Text without placeholders, args = null.", null); // Expected: MessageArgs == null

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[1]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[1]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedIds[1]);
      Assert.AreEqual("Text without placeholders, args = null.", CheapInMemorySink.CollectedMessageTemplates[1]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[1]);

      DevLogger.LogInformation(3, "Text without placeholders, args = {null}.", new object[] { null }); // Expected: MessageArgs == { null }

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[2]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[2]);
      Assert.AreEqual(3, CheapInMemorySink.CollectedIds[2]);
      Assert.AreEqual("Text without placeholders, args = {null}.", CheapInMemorySink.CollectedMessageTemplates[2]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[2][0]);

      object[] nullArray = null;
      DevLogger.LogInformation(4, "Text without placeholders, args = nullArray.", nullArray);// Expected: MessageArgs == null

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[3]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[3]);
      Assert.AreEqual(4, CheapInMemorySink.CollectedIds[3]);
      Assert.AreEqual("Text without placeholders, args = nullArray.", CheapInMemorySink.CollectedMessageTemplates[3]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[3]);

      string nullString = null;
      DevLogger.LogInformation(5, "Text without placeholders, args = nullString.", nullString); // Expected: MessageArgs == { null }

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[4]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[4]);
      Assert.AreEqual(5, CheapInMemorySink.CollectedIds[4]);
      Assert.AreEqual("Text without placeholders, args = nullString.", CheapInMemorySink.CollectedMessageTemplates[4]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[4][0]);

      DevLogger.LogInformation(6, "Text without placeholders, args = null, null.", null, null); // Expected: MessageArgs == { null, null }

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[5]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[5]);
      Assert.AreEqual(6, CheapInMemorySink.CollectedIds[5]);
      Assert.AreEqual("Text without placeholders, args = null, null.", CheapInMemorySink.CollectedMessageTemplates[5]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[5][0]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[5][1]);

      InfrastructureLogger.LogWarning(7, "{thingy} space is low: {space} MB", "Disk", 5);

      Assert.AreEqual("Ins", CheapInMemorySink.CollectedChannels[6]);
      Assert.AreEqual(3, CheapInMemorySink.CollectedLevels[6]);
      Assert.AreEqual(7, CheapInMemorySink.CollectedIds[6]);
      Assert.AreEqual("{thingy} space is low: {space} MB", CheapInMemorySink.CollectedMessageTemplates[6]);
      Assert.AreEqual("Disk", CheapInMemorySink.CollectedMessageArgs[6][0]);
      Assert.AreEqual(5, CheapInMemorySink.CollectedMessageArgs[6][1]);

      ProtocolLogger.LogError(
        8,
        "User \"{UserLogonName}\" does not have sufficient rights to perform \"{Interaction}\" on environment \"{Environment}\".",
        "Müller", "Delete", "Productive"
      );

      Assert.AreEqual("Pro", CheapInMemorySink.CollectedChannels[7]);
      Assert.AreEqual(4, CheapInMemorySink.CollectedLevels[7]);
      Assert.AreEqual(8, CheapInMemorySink.CollectedIds[7]);
      Assert.AreEqual("User \"{UserLogonName}\" does not have sufficient rights to perform \"{Interaction}\" on environment \"{Environment}\".", CheapInMemorySink.CollectedMessageTemplates[7]);
      Assert.AreEqual("Müller", CheapInMemorySink.CollectedMessageArgs[7][0]);
      Assert.AreEqual("Delete", CheapInMemorySink.CollectedMessageArgs[7][1]);
      Assert.AreEqual("Productive", CheapInMemorySink.CollectedMessageArgs[7][2]);

      DevLogger.LogCritical(-12345, null, null);

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[8]);
      Assert.AreEqual(5, CheapInMemorySink.CollectedLevels[8]); // Critical
      Assert.AreEqual(-12345, CheapInMemorySink.CollectedIds[8]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageTemplates[8]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[8]);

      Assert.AreEqual(this.ExpectedPipeKind, CheapInMemorySink.CollectedPipes[0]);

      Assert.AreEqual(9, CheapInMemorySink.CollectedIds.Count);
    }

    [TestMethod()]
    public void LogReturnCodeAs_Codes_TranslateCorrectlyToLevels() {

      DevLogger.LogReturnCodeAsError(123, "Simple with {placeholder}.", new[] { "resolved placeholder" });

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[0]);
      Assert.AreEqual(2, CheapInMemorySink.CollectedLevels[0]); // Info, because returnCode was positive
      Assert.AreEqual(123, CheapInMemorySink.CollectedIds[0]);
      Assert.AreEqual("Simple with {placeholder}.", CheapInMemorySink.CollectedMessageTemplates[0]);
      Assert.AreEqual("resolved placeholder", CheapInMemorySink.CollectedMessageArgs[0][0]);

      DevLogger.LogReturnCodeAsError(-456, "Error with args = null.", null);

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[1]);
      Assert.AreEqual(4, CheapInMemorySink.CollectedLevels[1]); // Error, because returnCode was negative
      Assert.AreEqual(456, CheapInMemorySink.CollectedIds[1]); // ID must (always) be positive
      Assert.AreEqual("Error with args = null.", CheapInMemorySink.CollectedMessageTemplates[1]);
      Assert.IsNull(CheapInMemorySink.CollectedMessageArgs[1]);

      DevLogger.LogReturnCodeAsWarning(-789, "Warning with empty placeholder array.", Array.Empty<object>());

      Assert.AreEqual("Dev", CheapInMemorySink.CollectedChannels[2]);
      Assert.AreEqual(3, CheapInMemorySink.CollectedLevels[2]); // Warning, because returnCode was negative
      Assert.AreEqual(789, CheapInMemorySink.CollectedIds[2]); // ID must (always) be positive
      Assert.AreEqual("Warning with empty placeholder array.", CheapInMemorySink.CollectedMessageTemplates[2]);
      Assert.AreEqual(0, CheapInMemorySink.CollectedMessageArgs[2].Length);

      Assert.AreEqual(3, CheapInMemorySink.CollectedIds.Count);
    }

  }
}
