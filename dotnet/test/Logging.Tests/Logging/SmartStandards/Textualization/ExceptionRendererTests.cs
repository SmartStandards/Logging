using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Logging.SmartStandards {

  [TestClass()]
  public class ExceptionRendererTests {

    [TestMethod()]
    public void Render_BasicExample_ReadsAsExpected() {

      Exception caughtException = CreateMockException();

      string renderedException = ExceptionRenderer.Render(caughtException);

      /*
            Fehler aus der BL >> Id ist ungültig! (Parameter 'Id')
            __System.ApplicationException__
            @   at Logging.SmartStandards.ExceptionSerializerTests.CreateMockException()
            @   C:\TKP\(git)\SmartStandards.Logging\dotnet\test\Logging.Tests\Logging\SmartStandards\ExceptionSerializerTests.cs:line 50
            __System.ArgumentException__ (inner)
            @   at Logging.SmartStandards.ExceptionSerializerTests.ThrowDeepException(Int32 depth)
            @   C:\TKP\(git)\SmartStandards.Logging\dotnet\test\Logging.Tests\Logging\SmartStandards\ExceptionSerializerTests.cs:line 61
            @   at Logging.SmartStandards.ExceptionSerializerTests.ThrowDeepException(Int32 depth)
            @   C:\TKP\(git)\SmartStandards.Logging\dotnet\test\Logging.Tests\Logging\SmartStandards\ExceptionSerializerTests.cs:line 63
            @   at Logging.SmartStandards.ExceptionSerializerTests.ThrowDeepException(Int32 depth)
            @   C:\TKP\(git)\SmartStandards.Logging\dotnet\test\Logging.Tests\Logging\SmartStandards\ExceptionSerializerTests.cs:line 63
            @   at Logging.SmartStandards.ExceptionSerializerTests.CreateMockException()
            @   C:\TKP\(git)\SmartStandards.Logging\dotnet\test\Logging.Tests\Logging\SmartStandards\ExceptionSerializerTests.cs:line 46
       */

    }

    internal static Exception CreateMockException() {
      Exception caughtException = null;
      try {
        try {
          ThrowDeepException(3);
          //throw new ArgumentException("Id ist ungültig!", "Id");
        } catch (Exception innerEx) {
          throw new ApplicationException("Fehler aus der BL", innerEx);
        }
      } catch (Exception ex) {
        caughtException = ex;
      }
      return caughtException;
    }

    internal static void ThrowDeepException(int depth) {
      if (depth == 1) {
        throw new ArgumentException("Id ist ungültig!", "Id");
      }
      ThrowDeepException(depth - 1);
    }

  }
}
