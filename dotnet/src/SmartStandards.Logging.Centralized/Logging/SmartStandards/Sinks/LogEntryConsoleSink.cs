using Logging.SmartStandards.Centralized;
using Logging.SmartStandards.Filtering;
using Logging.SmartStandards.Textualization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Logging.SmartStandards.Sinks {

  public class LogEntryConsoleSink : ILogEntrySink {

    public LogEntryConsoleSink() {
    }

    public void ReceiveLogEntries(LogEntry[] logEntries) {
      foreach (LogEntry logEntry in logEntries) {
        logEntry.ReEmitTo(ConsoleSink.WriteMessage);
      }
    }

    #region " Filter-Rules "

    private List<LogEntryFilteringRule> _FilteringRules = new List<LogEntryFilteringRule>();

    public IList<LogEntryFilteringRule> FilteringRules {
      get {
        return new List<LogEntryFilteringRule>();
      }
    }

    LogEntryFilteringRule[] ILogEntrySink.GetLogLevelFilteringRules() {
      lock (_FilteringRules) {
        return _FilteringRules.ToArray();
      }
    }

    #endregion

  }

}
