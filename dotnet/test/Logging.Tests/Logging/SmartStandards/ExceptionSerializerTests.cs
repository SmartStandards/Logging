using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Logging.SmartStandards.Internal;
using System.Reflection;

namespace Logging.SmartStandards {

  [TestClass()]
  public class ExceptionSerializerTests {

    [TestMethod()]
    public void ExceptionSerializerTest1() {

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

    [TestMethod()]
    public void GenericExceptionIdTest() {
      int genericId;

      genericId = ExceptionAnalyzer.InferEventIdByException(new ApplicationException("Foo"));
      Assert.AreEqual(863154666, genericId);

      genericId = ExceptionAnalyzer.InferEventIdByException(new ApplicationException("Bar"));
      Assert.AreEqual(863154666, genericId);

      genericId = ExceptionAnalyzer.InferEventIdByException(new Exception("Bar"));
      Assert.AreEqual(1969630032, genericId);

      genericId = ExceptionAnalyzer.InferEventIdByException(new ApplicationException("Foo #2233"));
      Assert.AreEqual(2233, genericId);

      genericId = ExceptionAnalyzer.InferEventIdByException(new TargetInvocationException(new ApplicationException("Foo #3344")));
      Assert.AreEqual(3344, genericId);

      genericId = ExceptionAnalyzer.InferEventIdByException(new Win32Exception(1122));
      Assert.AreEqual(1122, genericId);

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
