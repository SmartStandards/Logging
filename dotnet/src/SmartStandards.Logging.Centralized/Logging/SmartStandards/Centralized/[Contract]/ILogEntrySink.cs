using Logging.SmartStandards.Filtering;
using System;

namespace Logging.SmartStandards.Centralized {

  public interface ILogEntrySink {

    void ReceiveLogEntries(LogEntry[] logEntries);

    /// <summary>
    /// Returns an array of LogEntryFilteringRule,
    /// where the items are already sorted by their RuleOrder (acending).
    /// </summary>
    /// <returns></returns>
    LogEntryFilteringRule[] GetLogLevelFilteringRules();

  }

}
