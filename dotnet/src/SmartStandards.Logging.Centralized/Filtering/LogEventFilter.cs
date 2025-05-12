using Logging.SmartStandards.Centralized;
using System;
using System.Diagnostics;
using System.Linq;

namespace Logging.SmartStandards.Filtering {

  public class LogEventFilter {

    /// <summary> </summary>
    /// <param name="levelFilter">
    ///   The minimum level (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
    ///   that is required for a LogEvent, to be written into the sink.
    /// </param>
    public LogEventFilter(int levelFilter) {
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
    public LogEventFilter(int defaultLevelFilter, params LogEventFilteringRule[] filteringRules) {
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
    public LogEventFilter(int defaultLevelFilter, Func<LogEventFilteringRule[]> filteringRulesGetter) {
      this.DefaultLevelFilter = defaultLevelFilter;
      this.FilteringRulesGetter = filteringRulesGetter;
      this.FilterReloadIntervalSec = 180;
    }

    /// <summary>
    ///  A set of filtering rules (to reduce the amount of entries which should be redirected).
    /// </summary>
    /// <param name="filteringRules"></param>
    public void SetFilteringRules(params LogEventFilteringRule[] filteringRules) {
      if (filteringRules != null) {
        filteringRules = Array.Empty<LogEventFilteringRule>();
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
    public Func<LogEventFilteringRule[]> FilteringRulesGetter { get; set; } = () => Array.Empty<LogEventFilteringRule>();

    /// <summary>
    /// Configures the invalidation-timespan (in seconds) after which the 'FilteringRulesGetter' will be invoked again.
    /// </summary>
    public int FilterReloadIntervalSec { get; set; } = 180;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DateTime _FilterCacheValidUntil = DateTime.MinValue;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private LogEventFilteringRule[] _CurrentFilteringRules = null;

    private LogEventFilteringRule[] GetOrReloadCurrentFilteringRules() {

      if (_CurrentFilteringRules == null || _FilterCacheValidUntil < DateTime.Now) {
        if (this.FilterReloadIntervalSec < 1) {
          _FilterCacheValidUntil = DateTime.MaxValue;
        }
        else {
          _FilterCacheValidUntil = DateTime.Now.AddSeconds(this.FilterReloadIntervalSec);
        }
        if (this.FilteringRulesGetter == null) {
          this.FilteringRulesGetter = () => Array.Empty<LogEventFilteringRule>();
        }
        try {
          _CurrentFilteringRules = this.FilteringRulesGetter.Invoke();
          if (_CurrentFilteringRules == null) {
            _CurrentFilteringRules = Array.Empty<LogEventFilteringRule>();
          }
          else {
            //sicherheitshalber nochmal sortieren
            _CurrentFilteringRules = _CurrentFilteringRules.OrderBy((r) => r.RuleOrder).ToArray();
          }
        }
        catch (Exception ex) {
          _CurrentFilteringRules = Array.Empty<LogEventFilteringRule>();
          InsLogger.LogCritical(2073963765932919427, ex.Wrap($"The '{nameof(LogEventFilter)}' failed to (re)load the {nameof(LogEventFilteringRule)}s!"));
          if (this.FilterReloadIntervalSec < 1) {
            _FilterCacheValidUntil = DateTime.Now.AddSeconds(60);
          }
        }
      }
      return _CurrentFilteringRules;
    }

    public bool IsMatch(LogEvent logEvent) {
      LogEventFilteringRule[] rules = this.GetOrReloadCurrentFilteringRules();

      foreach (var rule in rules) {

        if(rule.SourceContext != "*" && !rule.SourceContext.Equals(logEvent.SourceContext, StringComparison.InvariantCultureIgnoreCase)) {
          continue;
        }
        if (rule.Application != "*" && !rule.SourceContext.Equals(logEvent.SourceContext, StringComparison.InvariantCultureIgnoreCase)) {
          continue;
        }
        if (rule.Audience != "*" && !rule.SourceContext.Equals(logEvent.SourceContext, StringComparison.InvariantCultureIgnoreCase)) {
          continue;
        }
        if (rule.InfrastructureZone != "*" && !rule.SourceContext.Equals(logEvent.SourceContext, StringComparison.InvariantCultureIgnoreCase)) {
          continue;
        }

        foreach (var kvp in rule.CustomFieldFilters) {
          if(logEvent.CustomFields.TryGetValue(kvp.Key,out object value)) {
            if (!kvp.Value.Equals(value)) {
              continue;
            }
          }
        }

        return (logEvent.Level >= rule.Level);
      }

      return (logEvent.Level >= this.DefaultLevelFilter);
    }

  }

}
