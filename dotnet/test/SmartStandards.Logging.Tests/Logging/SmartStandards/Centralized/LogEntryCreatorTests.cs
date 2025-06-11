using Logging.SmartStandards.Filtering;
using Logging.SmartStandards.Sinks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Logging.SmartStandards.Centralized {

  [TestClass]
  public class LogEntryCreatorTests {

    [TestMethod()]
    public void LogEntryCreator_Buffering_ShouldWork() {

      List<LogEntry> entriesOfLastFlush = new List<LogEntry>();

      Action<LogEntry[]> bufferCallback = (entries) => {
        entriesOfLastFlush.Clear();
        foreach (LogEntry entry in entries) {
          entriesOfLastFlush.Add(entry);
        }
      };

      MockSink sink = new MockSink(bufferCallback);

      int autoFlushMaxAge = 2; //s
      int autoFlushMaxCount = 2;
      int defaultLevelFilter = 3;
      int filterReloadIntervalSec = -1;

      LogEntryCreator.AddSink(sink, autoFlushMaxAge, autoFlushMaxCount, defaultLevelFilter, filterReloadIntervalSec);

      //#### EXPERIMENTAL ####

      //var fs = new LogEntryFileSink(".\\Logs\\test-{timestamp:yyyy-MM-dd-ss}--{application}-{audience}.log");
      //LogEntryCreator.AddSink(fs, autoFlushMaxAge, autoFlushMaxCount, 3, filterReloadIntervalSec);

      //var cs = new LogEntryConsoleSink();
      //LogEntryCreator.AddSink(cs, autoFlushMaxAge, autoFlushMaxCount, 3, filterReloadIntervalSec);

      //######################

      LogEntryCreator.ProcessMessage(DevLogger.AudienceToken, 3, "MySourceContext", 0, 123, "I think {Foo} is very good!", new object[] { "Bar" });

      Assert.AreEqual(0, entriesOfLastFlush.Count, "There should not yet be any entry in the buffer.");
      Thread.Sleep(3000); // wait for the auto-flush
      Assert.AreEqual(1, entriesOfLastFlush.Count, "There should be one entry in the buffer.");

      entriesOfLastFlush.Clear();

      LogEntryCreator.ProcessMessage(DevLogger.AudienceToken, 3, "MySourceContext", 0, 123, "I think {Foo} is very good!", new object[] { "Bar" });

      Assert.AreEqual(0, entriesOfLastFlush.Count, "There should not yet be any entry in the buffer.");

      LogEntryCreator.ProcessMessage(DevLogger.AudienceToken, 3, "MySourceContext", 0, 123, "I think {Foo} is very good!", new object[] { "Bar" });

      Thread.Sleep(350);//the internal sleep-time of the auto-flush awaiter-loop

      Assert.AreEqual(2, entriesOfLastFlush.Count, "There should be two entries in the buffer.");

    }

    private class MockSink : ILogEntrySink {

      private Action<LogEntry[]> _Callback;

      public MockSink(Action<LogEntry[]> callback) {
        _Callback = callback;
      }

      public void ReceiveLogEntries(LogEntry[] logEntries) {
        _Callback.Invoke(logEntries);
      }

      LogEntryFilteringRule[] ILogEntrySink.GetLogLevelFilteringRules() {
        return Array.Empty<LogEntryFilteringRule>();
      }

    }

  }

}
