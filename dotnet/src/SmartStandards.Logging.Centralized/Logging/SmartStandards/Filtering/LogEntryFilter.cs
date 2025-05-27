using Logging.SmartStandards.Centralized;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Logging.SmartStandards.Filtering {

  public class LogEntryFilter {

    /// <summary> </summary>
    /// <param name="levelFilter">
    ///   The minimum level (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
    ///   that is required for a LogEvent, to be written into the sink.
    /// </param>
    public LogEntryFilter(int levelFilter) {
      this.DefaultLevelFilter = levelFilter;
      this.SetFilteringRules();
    }

    /// <summary> </summary>
    /// <param name="defaultLevelFilter">
    ///   The minimum level (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
    ///   that is required for a LogEvent, to be written into the sink,
    ///   if the LogEvent's metadata does not match to any of the provided LogEventFilteringRule.
    /// </param>
    /// <param name="filteringRules">
    ///  A set of filtering rules (to reduce the amount of entries which should be redirected).
    /// </param>
    public LogEntryFilter(int defaultLevelFilter, params LogEntryFilteringRule[] filteringRules) {
      this.DefaultLevelFilter = defaultLevelFilter;
      this.SetFilteringRules(filteringRules);
    }

    /// <summary> 
    /// NOTE: use this overload, if you want that the filtering rules will be reloaded periodically (see also the 'FilterReloadIntervalSec'-Property)
    /// </summary>
    /// <param name="defaultLevelFilter">
    ///   The minimum level (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
    ///   that is required for a LogEvent, to be written into the sink,
    ///   if the LogEvent's metadata does not match to any of the provided LogEventFilteringRule.
    /// </param>
    /// <param name="filteringRulesGetter">
    ///  A method, that returns a set of filtering rules (to reduce the amount of entries which should be redirected).
    /// </param>
    public LogEntryFilter(int defaultLevelFilter, Func<LogEntryFilteringRule[]> filteringRulesGetter) {
      this.DefaultLevelFilter = defaultLevelFilter;
      this.FilteringRulesGetter = filteringRulesGetter;
      this.FilterReloadIntervalSec = 180;
    }

    /// <summary>
    ///  A set of filtering rules (to reduce the amount of entries which should be redirected).
    /// </summary>
    /// <param name="filteringRules"></param>
    public void SetFilteringRules(params LogEntryFilteringRule[] filteringRules) {
      if (filteringRules != null) {
        filteringRules = Array.Empty<LogEntryFilteringRule>();
      }
      this.FilteringRulesGetter = () => filteringRules;
      this.FilterReloadIntervalSec = -1;
      this.GetOrReloadCurrentFilteringRules();
    }

    /// <summary>
    ///   The minimum level (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
    ///   that is required for a LogEvent, to be written into the sink.
    ///   This acts as fallback after any FilteringRules (if provided).
    /// </summary>
    public int DefaultLevelFilter { get; set; } = 2; //info

    /// <summary>
    ///  A method, that returns a set of filtering rules (to reduce the amount of entries which should be redirected)
    /// </summary>
    public Func<LogEntryFilteringRule[]> FilteringRulesGetter { get; set; } = () => Array.Empty<LogEntryFilteringRule>();

    /// <summary>
    /// Configures the invalidation-timespan (in seconds) after which the 'FilteringRulesGetter' will be invoked again.
    /// </summary>
    public int FilterReloadIntervalSec { get; set; } = 180;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DateTime _FilterCacheValidUntil = DateTime.MinValue;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private LogEntryFilteringRule[] _CurrentFilteringRules = null;

    private LogEntryFilteringRule[] GetOrReloadCurrentFilteringRules() {

      if (_CurrentFilteringRules == null || _FilterCacheValidUntil < DateTime.Now) {
        if (this.FilterReloadIntervalSec < 1) {
          _FilterCacheValidUntil = DateTime.MaxValue;
        }
        else {
          _FilterCacheValidUntil = DateTime.Now.AddSeconds(this.FilterReloadIntervalSec);
        }
        if (this.FilteringRulesGetter == null) {
          this.FilteringRulesGetter = () => Array.Empty<LogEntryFilteringRule>();
        }
        try {
          _CurrentFilteringRules = this.FilteringRulesGetter.Invoke();
          if (_CurrentFilteringRules == null) {
            _CurrentFilteringRules = Array.Empty<LogEntryFilteringRule>();
          }
          else {
            //sicherheitshalber nochmal sortieren
            _CurrentFilteringRules = _CurrentFilteringRules.OrderBy((r) => r.RuleOrder).ToArray();
          }
        }
        catch (Exception ex) {
          _CurrentFilteringRules = Array.Empty<LogEntryFilteringRule>();
          InsLogger.LogCritical(2073963765932919427, ex.Wrap($"The '{nameof(LogEntryFilter)}' failed to (re)load the {nameof(LogEntryFilteringRule)}s!"));
          if (this.FilterReloadIntervalSec < 1) {
            _FilterCacheValidUntil = DateTime.Now.AddSeconds(60);
          }
        }
      }
      return _CurrentFilteringRules;
    }

    public bool IsMatch(LogEntry logEntry) {
      LogEntryFilteringRule[] rules = this.GetOrReloadCurrentFilteringRules();

      foreach (var rule in rules) {

        if (rule.Audience != "*" && !rule.Audience.Equals(logEntry.Audience, StringComparison.InvariantCultureIgnoreCase)) {
          continue;
        }
        if (rule.SourceContext != "*" && !rule.SourceContext.Equals(logEntry.SourceContext, StringComparison.InvariantCultureIgnoreCase)) {
          continue;
        }

        if (!HasMatchingField(logEntry.CustomFields, nameof(rule.Application), rule.Application)) {
          continue;
        }

        return (logEntry.Level >= rule.Level);
      }

      return (logEntry.Level >= this.DefaultLevelFilter);
    }

    private static bool HasMatchingField(Dictionary<string, string> fields, string fieldName, string expectedValue) {

      if (expectedValue == "*") {
        return true;
      }

      if (fields != null) {
        foreach (var kvp in fields) {
          //move 'Application' into explicit field
          if (kvp.Key.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase)) {
            return (expectedValue.Equals(kvp.Value, StringComparison.CurrentCultureIgnoreCase));
          }
        }
      }

      return false;
    }

  }

}
