using Logging.SmartStandards.Filtering;
using System;

namespace Logging.SmartStandards.Configuration {

  /// <summary>
  /// A Settings-Section to be deserialized from a configuration-file
  /// </summary>
  public class LoggingConfiguration {

    public ConsoleLoggingConfiguration ConsoleLogging { get; set; } = null;

    public FileLoggingConfiguration FileLogging { get; set; } = null;

    public RemoteLoggingConfiguration RemoteLogging { get; set; } = null;

    /// <summary>
    /// Names of additional (ambient-)metadata which should be added to the log entires.
    /// For example "SystemName" "InfrastructureZone" "PortfolioName" or any additinal information
    /// corresponding to the current application-scope.
    /// </summary>
    public string[] Enrichments { get; set; } = Array.Empty<string>();

  }

  public class ConsoleLoggingConfiguration {

    public bool Enabled { get; set; } = true;

    public int DefaultLogLevelFilter { get; set; } = 3;

    public LogEntryFilteringRule[] LogLevelRules { get; set; } = Array.Empty<LogEntryFilteringRule>();

  }

  public class FileLoggingConfiguration {

    public bool Enabled { get; set; } = true;

    public string TargetFileName { get; set; } = null;

    public int DefaultLogLevelFilter { get; set; } = 3;

    public LogEntryFilteringRule[] LogLevelRules { get; set; } = Array.Empty<LogEntryFilteringRule>();

    public int BufferMaxAgeSec { get; set; } = 10;

    public int BufferMaxCount { get; set; } = 100;

  }

  public class RemoteLoggingConfiguration {

    public bool Enabled { get; set; } = true;

    public string TargetSinkUrl { get; set; } = null;

    public string AuthHeader { get; set; } = null;

    public int DefaultLogLevelFilter { get; set; } = 3;

    public LogEntryFilteringRule[] LogLevelRules { get; set; } = Array.Empty<LogEntryFilteringRule>();

    public int FilterReloadIntervalSec { get; set; } = 180;

    public int BufferMaxAgeSec { get; set; } = 10;

    public int BufferMaxCount { get; set; } = 100;

  }

}
