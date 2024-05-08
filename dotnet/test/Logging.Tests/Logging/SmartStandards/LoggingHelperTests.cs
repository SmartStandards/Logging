using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  [TestClass()]
  public class LoggingHelperTests {

    [TestMethod()]
    public void StatusToFormattedLogEntry_VariousTestPatterns_ProduceExpectedResults() {

      string formattedLogEntry;

      formattedLogEntry = LoggingHelper.StatusToFormattedLogEntry(
        0, 4711, "Hello {audience}, the answer is {answer}.", new object[] { "world", 42 }
      );
      Assert.AreEqual("[Trc]4711:Hello world, the answer is 42.", formattedLogEntry);

      formattedLogEntry = LoggingHelper.StatusToFormattedLogEntry(
        1, 4711, "Hello {audience}, the answer is {answer}.", new object[] { "world", 42 }
      );
      Assert.AreEqual("[Dbg]4711:Hello world, the answer is 42.", formattedLogEntry);

      formattedLogEntry = LoggingHelper.StatusToFormattedLogEntry(
        2, 4711, "Hello {audience}, the answer is {answer}.", new object[] { "world", 42 }
      );
      Assert.AreEqual("[Inf]4711:Hello world, the answer is 42.", formattedLogEntry);

      formattedLogEntry = LoggingHelper.StatusToFormattedLogEntry(
        3, 4711, "Hello {audience}, the answer is {answer}.", new object[] { "world", 42 }
      );
      Assert.AreEqual("[Wrn]4711:Hello world, the answer is 42.", formattedLogEntry);

      formattedLogEntry = LoggingHelper.StatusToFormattedLogEntry(
        4, 4711, "Hello {audience}, the answer is {answer}.", new object[] { "world", 42 }
      );
      Assert.AreEqual("[Err]4711:Hello world, the answer is 42.", formattedLogEntry);

      formattedLogEntry = LoggingHelper.StatusToFormattedLogEntry(
        5, 4711, "Hello {audience}, the answer is {answer}.", new object[] { "world", 42 }
      );
      Assert.AreEqual("[Cri]4711:Hello world, the answer is 42.", formattedLogEntry);

      // Invalid levels => fallback to [Cri]

      formattedLogEntry = LoggingHelper.StatusToFormattedLogEntry(
        -1, 4711, "Hello {audience}, the answer is {answer}.", new object[] { "world", 42 }
      );
      Assert.AreEqual("[Cri]4711:Hello world, the answer is 42.", formattedLogEntry);

      formattedLogEntry = LoggingHelper.StatusToFormattedLogEntry(
        6, 4711, "Hello {audience}, the answer is {answer}.", new object[] { "world", 42 }
      );
      Assert.AreEqual("[Cri]4711:Hello world, the answer is 42.", formattedLogEntry);


    }

  }

}
