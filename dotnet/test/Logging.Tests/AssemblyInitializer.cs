using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.Tests {

  [TestClass]
  public class AssemblyInitializer {

    public const string SourceContext = "Logging.Tests";

    [AssemblyInitialize]
    public static void InitializeAssembly(TestContext testContext) {

      MockingTraceListener.Initialize(null);
    }
  }
}
