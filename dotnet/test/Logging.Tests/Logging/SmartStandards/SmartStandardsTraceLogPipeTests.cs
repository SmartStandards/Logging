using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Discovery;
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
      try {

        int receivedCounter = 0;
        string capturedMessageFromLogMethod = null;
        DevLogger.ConfigureRedirection(
          (string c, int l, int id, string msg, object[] args) => {
            capturedMessageFromLogMethod = msg;
            receivedCounter++;
          },
          logExceptionMethod: null,
          forwardDirectInputToTracing: true, // <<<<<<<<<<< DIRECT input
          forwardTracingInputToLogMehod: true // <<<<<<<<< TRACING input
        );

        //Test DIRECT input
        DevLogger.LogWarning(0, "AAA");

        Assert.AreEqual("AAA", capturedMessageFromLogMethod); //< pass-trough
        Assert.AreEqual("AAA", UnitTestTraceListener.LastMessageTemplate);

        //loopback-detection: we should only get that one message, which was piped directly, but not 
        //that one which was received via trace-listener (after we've passed it to trace as additional output)
        Assert.AreEqual(1, receivedCounter);

        //Test indirect TRACING input
        LogToTraceAdapter.LogToTrace("Dev", 3, 0, "BBB");

        Assert.AreEqual("BBB", capturedMessageFromLogMethod);
        Assert.AreEqual("BBB", UnitTestTraceListener.LastMessageTemplate);

      }
      finally {
        UnitTestTraceListener.Terminate();
      }
    }

    [TestMethod()]
    public void LogPipeExceptionTest() {
      UnitTestTraceListener.EnsureIsInitialized();
      try {

        Exception capturedExceptionFromLogMethod = null;

        DevLogger.ConfigureRedirection(
          (string c, int l, int id, string msg, object[] args) => { },
          (string c, int l, int id, Exception ex) => {
            capturedExceptionFromLogMethod = ex;
          },
          forwardTracingInputToLogMehod: true
        );

        Exception ex = new Exception("Kaputt");

        //Test indirect TRACING input
        LogToTraceAdapter.LogToTrace("Dev", 3, 0, ex.Serialize(), new Exception("Kaputt"));

        Assert.AreEqual("Kaputt\r\n__System.Exception__", UnitTestTraceListener.LastMessageTemplate);
        Assert.AreEqual(1, UnitTestTraceListener.LastArgs.Length);

        Assert.IsNotNull(capturedExceptionFromLogMethod);
        Assert.AreEqual("Kaputt", capturedExceptionFromLogMethod.Message);

      }
      finally {
        UnitTestTraceListener.Terminate();
      }
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
