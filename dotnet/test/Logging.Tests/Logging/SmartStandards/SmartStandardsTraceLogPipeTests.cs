using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.SmartStandards {

  [TestClass()]
  public class SmartStandardsTraceLogPipeTests {

    [TestMethod()]
    public void LogPipeCrossTargetTest() {

      UnitTestTraceListener.EnsureIsInitialized();

      string capturedMessageFromLogMethod = null;
      DevLogger.ConfigureRedirection(
        (string c, int l, int id, string msg, object[] args) => {
          capturedMessageFromLogMethod = msg;
        },
        logExceptionMethod: null,
        forwardDirectInputToTracing: true, // <<<<<<<<<<< DIRECT input
        forwardTracingInputToLogMehod: true // <<<<<<<<< TRACING input
      ); 

      //Test DIRECT input
      DevLogger.LogWarning(0, "AAA");

      Assert.AreEqual("AAA", capturedMessageFromLogMethod); //< pass-trough
      Assert.AreEqual("AAA", UnitTestTraceListener.LastMessageTemplate);

      //Test indirect TRACING input
      LogToTraceAdapter.LogToTrace("Dev", 3, 0, "BBB");

      Assert.AreEqual("BBB", capturedMessageFromLogMethod);

    }

    [TestMethod()]
    public void LogPipeExceptionTest() {

      UnitTestTraceListener.EnsureIsInitialized();

      Exception capturedExceptionFromLogMethod = null;

      DevLogger.ConfigureRedirection(
        null,
        (string c, int l, int id, Exception ex) => {
          capturedExceptionFromLogMethod = ex;
        },
        forwardTracingInputToLogMehod: true
      );


      //Test indirect TRACING input
      LogToTraceAdapter.LogToTrace("Dev", 3, 0, "An Exception", new Exception("Kaputt"));

      Assert.AreEqual("An Exception", UnitTestTraceListener.LastMessageTemplate);
      Assert.AreEqual(1, UnitTestTraceListener.LastArgs.Length);

      Assert.IsNotNull(capturedExceptionFromLogMethod);
      Assert.AreEqual("Kaputt", capturedExceptionFromLogMethod.Message);

    }

    //[TestMethod()]
    //public void LogPipeLoadSimulation() {

    //  string capturedMessage;
    //  int receivedMessages = 0;
    //  SmartStandardsTraceLogPipe.InitializeAsLoggerInput();
    //  DevLogger.ConfigureRedirection(
    //    (string c, int l, int id, string msg, object[] args) => {
    //      capturedMessage = msg;
    //      receivedMessages++;
    //    }, null, false, false);

    //  LogToTraceAdapter.LogToTrace("Dev", 3, 0, ExceptionSerializerTests.CreateMockException().Serialize());

    //  string msg = ExceptionSerializerTests.CreateMockException().Serialize();
    //  for (int i = 0; i < 30; i++) {
    //    Task.Run(() => {
    //      Thread.Sleep(1000);
    //      LogToTraceAdapter.LogToTrace("Dev", 3, 0, msg);
    //    });
    //  }
    //}

  }

}
