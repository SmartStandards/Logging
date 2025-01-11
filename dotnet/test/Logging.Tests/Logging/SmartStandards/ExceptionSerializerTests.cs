using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  [TestClass()]
  public class ExceptionSerializerTests {

    [TestMethod()]
    public void ExceptionSerializerTest1() {

      Exception catchedEx = CreateMockException();

      string serializedEx = catchedEx.Serialize();

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

      genericId = ExceptionSerializer.GetGenericIdFromException(new ApplicationException("Foo"));
      Assert.AreEqual(863154666, genericId);

      genericId = ExceptionSerializer.GetGenericIdFromException(new ApplicationException("Bar"));
      Assert.AreEqual(863154666, genericId);

      genericId = ExceptionSerializer.GetGenericIdFromException(new Exception("Bar"));
      Assert.AreEqual(1969630032, genericId);

      genericId = ExceptionSerializer.GetGenericIdFromException(new ApplicationException("Foo #2233"));
      Assert.AreEqual(2233, genericId);

      genericId = ExceptionSerializer.GetGenericIdFromException(new TargetInvocationException(new ApplicationException("Foo #3344")));
      Assert.AreEqual(3344, genericId);

      genericId = ExceptionSerializer.GetGenericIdFromException(new Win32Exception(1122));
      Assert.AreEqual(1122, genericId);

    }

    internal static Exception CreateMockException() {
      Exception catchedEx = null;
      try {
        try {
          ThrowDeepException(3);
          //throw new ArgumentException("Id ist ungültig!", "Id");
        } catch (Exception innerEx) {
          throw new ApplicationException("Fehler aus der BL", innerEx);
        }
      } catch (Exception ex) {
        catchedEx = ex;
      }
      return catchedEx;
    }

    internal static void ThrowDeepException(int depth) {
      if (depth == 1) {
        throw new ArgumentException("Id ist ungültig!", "Id");
      }
      ThrowDeepException(depth - 1);
    }

  }

}
