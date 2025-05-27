using System.Collections.Generic;

namespace Logging.SmartStandards.Filtering {

  public class LogEntryFilteringRule {

    /// <summary>
    /// LogLevelConfigurationRule's will be evaluated in ascending RuleOrder - 
    /// FIRST MATCH WINNS!
    /// </summary>
    public int RuleOrder { get; set; }

    /// <summary> 
    /// Minimum required level (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
    /// for Entries, matching the filtering-fields
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Filtering pattern for the 'Audience'-Channel ('Dev'|'Ins'|'Biz') of processed LogEntries (Wildcard = '*')
    /// </summary>
    public string Audience { get; set; } = "*";

    /// <summary>
    /// Filtering pattern for the 'SourceContext' of processed LogEntries (Wildcard = '*')
    /// </summary>
    public string SourceContext { get; set; } = "*";

    /// <summary>
    /// Filtering pattern for the 'Application' of processed LogEntries (Wildcard = '*')
    /// </summary>
    public string Application { get; set; } = "*";

    /// <summary>
    /// Filtering patterns for one or more custom fields of processed LogEntries (Wildcard = '*').
    /// (for Example another )
    /// </summary>
    public Dictionary<string, string> CustomFieldFilters { get; set; } = null;

  }

}
