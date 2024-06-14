using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.SmartStandards {

  [TestClass()]
  public class SmartStandardsTraceLogPipeTests {

    //[TestMethod()]
    //public void LogPipeLoadTest1() {

    //  string capturedMessage;
    //  int receivedMessages = 0;
    //  SmartStandardsTraceLogPipe.InitializeAsLoggerInput();
    //  DevLogger.LogMethod = (string c, int l, int id, string msg, object[] args) => {
    //    capturedMessage = msg;
    //    receivedMessages++;
    //  };

    //  LogToTraceAdapter.LogToTrace("Dev", 3,0, ExceptionSerializerTests.CreateMockException().Serialize());

    //  string msg = ExceptionSerializerTests.CreateMockException().Serialize();
    //  for(int i = 0;i < 30; i++) {
    //    Task.Run(() => { 
    //      Thread.Sleep(1000);
    //      LogToTraceAdapter.LogToTrace("Dev", 3, 0, msg);
    //    });
    //  }

    //  Thread.Sleep(2000);
    //}

  }

}
