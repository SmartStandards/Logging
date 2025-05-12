
export class LogEventFilteringRule {

  /// <summary>
  /// LogLevelConfigurationRule's will be evaluated in ascending RuleOrder - 
  /// FIRST MATCH WINNS!
  /// </summary>
  public RuleOrder: number = 0;

  /// <summary> 
  /// Minimum required level (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
  /// for Entries, matching the filtering-fields
  /// </summary>
  public Level: number = 0;

  /// <summary>
  /// Filtering pattern for the 'Audience'-Channel ('Dev'|'Ins'|'Biz') of processed LogEntries (Wildcard = '*')
  /// </summary>
  public Audience: string|null = "*";

  /// <summary>
  /// Filtering pattern for the 'SourceContext' of processed LogEntries (Wildcard = '*')
  /// </summary>
  public SourceContext: string | null = "*";

  /// <summary>
  /// Filtering pattern for the 'Application' of processed LogEntries (Wildcard = '*')
  /// </summary>
  public Application: string | null = "*";

  /// <summary>
  /// Filtering pattern for the 'InfrastructureZone' ('MyCompany/MyProduct/SytemName/...') of processed LogEntries (Wildcard = '*')
  /// </summary>
  public InfrastructureZone: string | null = "*";

  /// <summary>
  /// Filtering patterns for one or more custom fields of processed LogEntries (Wildcard = '*').
  /// (for Example another )
  /// </summary>
  public CustomFieldFilters: object|null = null;

}
