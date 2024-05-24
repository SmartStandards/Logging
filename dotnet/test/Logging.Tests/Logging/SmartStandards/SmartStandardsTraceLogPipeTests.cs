using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  [TestClass()]
  public class SmartStandardsTraceLogPipeTests : LoggerTestsBase {

    internal override bool UseTrace { get { return true; } }

  }
}
