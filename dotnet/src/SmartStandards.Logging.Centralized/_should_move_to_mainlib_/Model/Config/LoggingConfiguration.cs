using System;
using System.Collections.Generic;

namespace Logging.SmartStandards.Filtering {


  /// <summary>
  /// A Settings-Section to be deserialized from a configuration-file
  /// </summary>
  public class LoggingConfiguration {

    public FileLoggingConfiguration FileLogging { get; set; } = null;

    public RemoteLoggingConfiguration RemoteLogging { get; set; } = null;

  }

  public class FileLoggingConfiguration {

    public string TargetFileName { get; set; } = null;

    public int DefaultLogLevel { get; set; } = 3;

    public LogEventFilteringRule[] LogLevelRules { get; set; } = Array.Empty<LogEventFilteringRule>();

    /// <summary>
    /// Names of additional (ambient-)metadata which should be added to the log entires.
    /// For example "SystemName" "InfrastructureZone" "PortfolioName" or any additinal information
    /// corresponding to the current application-scope.
    /// </summary>
    public string[] Enrichments { get; set; } = Array.Empty<string>();

  }

  public class RemoteLoggingConfiguration {

    public string TargetSinkUrl { get; set; } = null;

    public string AuthHeader { get; set; } = null;

    /// <summary>
    /// Names of additional (ambient-)metadata which should be added to the log entires.
    /// For example "SystemName" "InfrastructureZone" "PortfolioName" or any additinal information
    /// corresponding to the current application-scope.
    /// </summary>
    public string[] Enrichments { get; set; } = Array.Empty<string>();

  }

}
