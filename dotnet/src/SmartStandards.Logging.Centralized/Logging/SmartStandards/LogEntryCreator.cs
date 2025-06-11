using Logging.SmartStandards.Centralized;
using Logging.SmartStandards.Configuration;
using Logging.SmartStandards.Sinks;
using Logging.SmartStandards.Textualization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.SmartStandards;

namespace Logging.SmartStandards {

  /// <summary>
  /// This class provides enhanced convenience for:
  /// Conversion to 'LogEntries', Enrichment (custom fields) and Buffering
  /// </summary>
  public static partial class LogEntryCreator {

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private static List<LogEntryDispatcher> _Dispatchers = new List<LogEntryDispatcher>();

    #region " Enrichment "

    public delegate void EnrichLogEventDelegate(IDictionary<string, string> customFields);

    public static EnrichLogEventDelegate EnrichVia { get; set; } = null;

    public static string ApplicationName { get; set; } = "Unknown";

    static LogEntryCreator() {
      try {
        Assembly entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null) {
          ApplicationName = entryAssembly.GetName().Name;
        }
        else if (Assembly.GetCallingAssembly() != null) {
          ApplicationName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule?.FileName);
        }   
      } catch { }
    }

    #endregion

    public static void ProcessMessage(
      string audience, int level, string sourceContext, long sourceLineId,
      int eventKindId, string messageTemplate, object[] args
    ) {

      LogEntry entry = CreateLogEntry(
        audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, args, null
      );

      lock (_Dispatchers) {
        foreach (LogEntryDispatcher dispatcher in _Dispatchers) {
          dispatcher.BufferEntry(entry);
        }
      }

    }

    public static void ProcessException(
      string audience, int level, string sourceContext, long sourceLineId,
      int eventKindId, Exception ex
    ) {

      var entry = CreateLogEntry(
        audience, level, sourceContext, sourceLineId, eventKindId, null, null, ex
      );

      lock (_Dispatchers) {
        foreach (LogEntryDispatcher dispatcher in _Dispatchers) {
          dispatcher.BufferEntry(entry);
        }
      }

    }

    public static LogEntry CreateLogEntry(
      string audience, int level, string sourceContext, long sourceLineId, int eventKindId, 
      string messageTemplate, object[] args,
      Exception ex
    ) {

      //map common properties, also present in LogEvent
      var entry = new LogEntry {
        Timestamp = Snowflake44.Generate(),
        Audience = audience,
        Level = level,
        SourceContext = sourceContext,
        SourceLineId = sourceLineId,
        EventKindId = eventKindId,
        MessageTemplate = messageTemplate,
        Application = ApplicationName,
        HostName = Environment.MachineName,
        CustomFields = new Dictionary<string, string>()
      };

      //convert exception to a serializes string representation
      if (ex != null) {
        entry.Exception = ExceptionRenderer.Render(ex, true);
      }

      if (EnrichVia != null) {
        EnrichVia.Invoke(entry.CustomFields);
      }

      //copy known Placeholder-Values (related to the MessageTemplate) into the CustomFields
      if (args != null && args.Length > 0 && messageTemplate != null) {
        int argIdx = 0;
        messageTemplate.ForEachPlaceholder(
          (string name) => {
            if (args.Length > argIdx) {
              entry.CustomFields[name] = args[argIdx]?.ToString();
              argIdx++;
              return false;
            }
            return true;
          }
        );
      }

      return entry;
    }

    public static void AddSink(ILogEntrySink sink) {
      lock (_Dispatchers) {
        var dispatcher = new LogEntryDispatcher(sink);
        _Dispatchers.Add(dispatcher);
      }
    }

    /// <summary></summary>
    /// <param name="sink"></param>
    /// <param name="autoFlushMaxAge">
    ///  Maximum timespan (in seconds) that a message will stay within the buffer
    ///  (default: 10 seconds)
    /// </param>
    /// <param name="autoFlushMaxCount">
    /// Maximum number of Messages within the buffer.
    /// (default: 100 messages)
    /// </param>
    /// <param name="defaultLevelFilter">
    ///   The minimum level (0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical)
    ///   that is required for a LogEvent, to be written into the sink.
    ///   This acts as fallback after any FilteringRules (if provided).
    /// </param>
    /// <param name="filterReloadIntervalSec">
    /// Configures the invalidation-timespan (in seconds) after which the 'FilteringRulesGetter' will be invoked again.
    /// </param>
    public static void AddSink(
      ILogEntrySink sink, 
      int autoFlushMaxAge = 10, int autoFlushMaxCount = 100, 
      int defaultLevelFilter = 2, int filterReloadIntervalSec = 180
    ) {
      lock (_Dispatchers) {
        var dispatcher = new LogEntryDispatcher(sink);
        dispatcher.AutoFlushMaxAge = autoFlushMaxAge;
        dispatcher.AutoFlushMaxCount = autoFlushMaxCount;
        dispatcher.Filter.DefaultLevelFilter = defaultLevelFilter;
        dispatcher.Filter.FilterReloadIntervalSec = filterReloadIntervalSec;
        _Dispatchers.Add(dispatcher);
      }
    }

    public static void ClearSinks() {
      lock (_Dispatchers) {
        _Dispatchers.Clear();
      }
    }

    public static void Flush() {
      lock (_Dispatchers) {
        foreach (var dispatcher in _Dispatchers) {
          dispatcher.FlushToSink();
        }
      }
    }

    #region " Setup (Convenience) "

    /// <summary>
    /// Configures the Routing (wire-up) to pass all log events to the LogEntryCreator (as 'CustomBus') and
    /// adds the given sinks.
    /// </summary>
    /// <param name="sinksToAdd"></param>
    public static void Setup(params ILogEntrySink[] sinksToAdd) {

      Routing.UseCustomBus(ProcessMessage, ProcessException);

      if (sinksToAdd != null) {
        foreach (var sink in sinksToAdd) {
          AddSink(sink);
        }
      }
    }

    /// <summary>
    /// Configures the Routing (wire-up) to pass all log events to the LogEntryCreator (as 'CustomBus'),
    /// configures the given enrichment-method and
    /// adds the given sinks.
    /// </summary>
    /// <param name="enrichVia"></param>
    /// <param name="sinksToAdd"></param>
    public static void Setup(EnrichLogEventDelegate enrichVia, params ILogEntrySink[] sinksToAdd) {
      EnrichVia = enrichVia;
      Setup(sinksToAdd);
    }

    /// <summary>
    /// Configures the Routing (wire-up) to pass all log events to the LogEntryCreator (as 'CustomBus') and
    /// initializes+adds sinks defined by the given LoggingConfiguration.
    /// </summary>
    /// <param name="configuration"></param>
    public static void Setup(LoggingConfiguration configuration) {

      if(configuration == null) {
        throw new ArgumentNullException($"{nameof(configuration)} must not be null!");
      }

      //wire-up the Routing to pass all log events to us (LogEntryCreator)
      Routing.UseCustomBus(ProcessMessage, ProcessException);

      //initialize and add the wellknown CONSOLE Sink
      if (configuration.ConsoleLogging != null && configuration.ConsoleLogging.Enabled) {
        ConsoleLoggingConfiguration cfg = configuration.ConsoleLogging;
        var sink = new LogEntryConsoleSink();
        if (cfg.LogLevelRules != null) {
          int ruleFallbackOrder = 0;
          foreach (var rule in cfg.LogLevelRules) {
            rule.RuleOrder = (rule.RuleOrder * 100) + ruleFallbackOrder;
            ruleFallbackOrder++;
            sink.FilteringRules.Add(rule);
          }
        }
        AddSink(sink, 0, 0, cfg.DefaultLogLevelFilter, -1);
      }

      //initialize and add the wellknown FILE Sink
      if (configuration.FileLogging != null && configuration.FileLogging.Enabled && !string.IsNullOrWhiteSpace(configuration.FileLogging.TargetFileName)) {
        FileLoggingConfiguration cfg = configuration.FileLogging;
        var sink = new LogEntryFileSink(cfg.TargetFileName);
        if (cfg.LogLevelRules != null) {
          int ruleFallbackOrder = 0;
          foreach (var rule in cfg.LogLevelRules) {
            rule.RuleOrder = (rule.RuleOrder * 100) + ruleFallbackOrder;
            ruleFallbackOrder++;
            sink.FilteringRules.Add(rule);
          }
        }
        AddSink(sink, cfg.BufferMaxAgeSec, cfg.BufferMaxCount, cfg.DefaultLogLevelFilter, -1);
      }

      //initialize and add the wellknown REMOTE Sink
      if (configuration.RemoteLogging != null && configuration.RemoteLogging.Enabled && !string.IsNullOrWhiteSpace(configuration.RemoteLogging.TargetSinkUrl)) {
        RemoteLoggingConfiguration cfg = configuration.RemoteLogging;
        var sink = new LogEntryRemoteSink(cfg.TargetSinkUrl);
        if (cfg.LogLevelRules != null) {
          int ruleFallbackOrder = 0;
          foreach (var rule in cfg.LogLevelRules) {
            rule.RuleOrder = (rule.RuleOrder * 100) + ruleFallbackOrder;
            ruleFallbackOrder++;
            sink.LocalFilteringRules.Add(rule);
          }
        }
        AddSink(sink, cfg.BufferMaxAgeSec, cfg.BufferMaxCount, cfg.DefaultLogLevelFilter, cfg.FilterReloadIntervalSec);
      }

    }

    /// <summary>
    /// Configures the Routing (wire-up) to pass all log events to the LogEntryCreator (as 'CustomBus'),
    /// configures the given enrichment-method and
    /// initializes+adds sinks defined by the given LoggingConfiguration.
    /// </summary>
    /// <param name="enrichVia"></param>
    /// <param name="configuration"></param>
    public static void Setup(EnrichLogEventDelegate enrichVia, LoggingConfiguration configuration) {
      EnrichVia = enrichVia;
      Setup(configuration);
    }

    #endregion

  }

}
