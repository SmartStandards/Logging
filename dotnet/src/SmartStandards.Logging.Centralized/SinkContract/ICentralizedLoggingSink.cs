using Logging.SmartStandards.Filtering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace Logging.SmartStandards.Centralized.SinkContract {

  public interface ICentralizedLoggingSink {

    void AdoptLogEvents(LogEvent[] events);

    [Obsolete("Overload for reduced LogEvents to be compaible with the LogEvent-class of the 'Serilog'-Framework, PLEASE CONSIDER TO USE THE FULL 'LogEvent' INSTEAD!")]
    void Add(LegacyLogEvent[] events);

    /// <summary>
    /// Returns an array of LogLevelFilteringRules,
    /// where the items are already sorted by their RuleOrder (acending).
    /// </summary>
    /// <returns></returns>
    LogEventFilteringRule[] GetLogLevelFilteringRules();

  }

}
