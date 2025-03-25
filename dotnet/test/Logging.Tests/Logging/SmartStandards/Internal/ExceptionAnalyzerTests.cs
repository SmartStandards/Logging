using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Logging.SmartStandards.Internal {

  [TestClass()]
  public class ExceptionAnalyzerTests {

    [TestMethod()]
    public void InferEventIdByException_DifferentTypes_ReturnMatchingIds() {

      int inferredId;

      inferredId = ExceptionAnalyzer.InferEventIdByException(new ApplicationException("Foo"));
      Assert.AreEqual(863154666, inferredId);

      inferredId = ExceptionAnalyzer.InferEventIdByException(new ApplicationException("Bar"));
      Assert.AreEqual(863154666, inferredId);

      inferredId = ExceptionAnalyzer.InferEventIdByException(new Exception("Bar"));
      Assert.AreEqual(1969630032, inferredId);

      inferredId = ExceptionAnalyzer.InferEventIdByException(new ApplicationException("Foo #2233"));
      Assert.AreEqual(2233, inferredId);

      inferredId = ExceptionAnalyzer.InferEventIdByException(new TargetInvocationException(new ApplicationException("Foo #3344")));
      Assert.AreEqual(3344, inferredId);

      inferredId = ExceptionAnalyzer.InferEventIdByException(new Win32Exception(1122));
      Assert.AreEqual(1122, inferredId);

    }

  }
}
