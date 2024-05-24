using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  [TestClass()]
  public class DevLoggerTests : LoggerTestsBase {

    internal override bool UseTrace { get { return false; } }

  }
}
