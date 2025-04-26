using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  [TestClass()]
  public class ExceptionRendererTests {

    [TestMethod()]
    public void Render_BasicExample_ReadsAsExpected() {

      Exception outerException = null;

      try {
        ThrowOuterException();
      } catch (Exception ex) {
        outerException = ex;
      }

      string renderedException = ExceptionRenderer.Render(outerException);

      /*
Outer message :: Middle message (Parameter 'Foo') :: Inner Message
-- System.ApplicationException --
@ Logging.SmartStandards.ExceptionRendererTests.Render_BasicExample_ReadsAsExpected()
@   C:\Git\Logging\dotnet\test\SmartStandards.Logging.Tests\Logging\SmartStandards\Textualization\ExceptionRendererTests.cs:line 15
@ Logging.SmartStandards.ExceptionRendererTests.ThrowOuterException()
@   C:\Git\Logging\dotnet\test\SmartStandards.Logging.Tests\Logging\SmartStandards\Textualization\ExceptionRendererTests.cs:line 44
-- System.ArgumentException --
@ Logging.SmartStandards.ExceptionRendererTests.ThrowOuterException()
@   C:\Git\Logging\dotnet\test\SmartStandards.Logging.Tests\Logging\SmartStandards\Textualization\ExceptionRendererTests.cs:line 42
@ Logging.SmartStandards.ExceptionRendererTests.ThrowMiddleException()
@   C:\Git\Logging\dotnet\test\SmartStandards.Logging.Tests\Logging\SmartStandards\Textualization\ExceptionRendererTests.cs:line 52
-- System.Exception --
@ Logging.SmartStandards.ExceptionRendererTests.ThrowMiddleException()
@   C:\Git\Logging\dotnet\test\SmartStandards.Logging.Tests\Logging\SmartStandards\Textualization\ExceptionRendererTests.cs:line 50
@ Logging.SmartStandards.ExceptionRendererTests.ThrowInnerException()
@   C:\Git\Logging\dotnet\test\SmartStandards.Logging.Tests\Logging\SmartStandards\Textualization\ExceptionRendererTests.cs:line 57
      */

    }

    internal static void ThrowOuterException() {
      try {
        ThrowMiddleException();
      } catch (Exception middleException) {
        throw new ApplicationException("Outer message", middleException);
      }
    }

    internal static void ThrowMiddleException() {
      try {
        ThrowInnerException();
      } catch (Exception innerException) {
        throw new ArgumentException("Middle message", "Foo", innerException);
      }
    }

    internal static void ThrowInnerException() {
      throw new Exception("Inner Message");
    }

  }
}
