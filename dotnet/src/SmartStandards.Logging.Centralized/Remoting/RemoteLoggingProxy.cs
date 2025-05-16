using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Logging.SmartStandards.Centralized.SinkContract;
using Logging.SmartStandards.Filtering;

namespace Logging.SmartStandards.Centralized {

  /// <summary>
  /// An (Server-Side) implementation of ICentralizedLoggingSink which easily can
  /// be exposed as service-endpoint to receive incomming LogEntries
  /// (usually comming from the 'RemoteSinkRedirector' at Client-Side)
  /// and forward them into the SmartStandars-Logger on server-side!
  /// </summary>
  public class RemoteLoggingProxy : ICentralizedLoggingSink {

    public void AdoptLogEvents(LogEvent[] events) {
      Task.Run((Action)(() => {
        foreach (LogEvent logEvent in events) {
          if (_Filter.IsMatch(logEvent)) {
            if (logEvent.Audience == BizLogger.AudienceToken) {
              BizLogger.Log(
                (int)logEvent.Level, logEvent.SourceContext, logEvent.SourceLineId, logEvent.UseCaseId,
                (string)logEvent.MessageTemplate
              );
            }
            else if (logEvent.Audience == InsLogger.AudienceToken) {
              InsLogger.Log(
                (int)logEvent.Level, logEvent.SourceContext, logEvent.SourceLineId, logEvent.UseCaseId,
                (string)logEvent.MessageTemplate
              );
            }
            else {
              DevLogger.Log(
                (int)logEvent.Level, logEvent.SourceContext, logEvent.SourceLineId, logEvent.UseCaseId,
                (string)logEvent.MessageTemplate
              );
            }
          }
        }
      }));
    }

    #region " Legacy-Overload (Serilog-Inspired) "

    [Obsolete("Overload for reduced LogEvents to be compaible with the LogEvent-class of the 'Serilog'-Framework, PLEASE CONSIDER TO USE THE FULL 'LogEvent' INSTEAD!")]
    public void Add(LegacyLogEvent[] events) {
      Task.Run(() => {
        foreach (LegacyLogEvent slogEvent in events) {

          //map to new structure!
          LogEvent logEvent  = slogEvent.ToLogEvent();

          if (_Filter.IsMatch(logEvent)) {
            if (logEvent.Audience == BizLogger.AudienceToken) {
              BizLogger.Log(
                logEvent.Level, logEvent.SourceContext, logEvent.SourceLineId, logEvent.UseCaseId,
                logEvent.MessageTemplate
              );
            }
            else if (logEvent.Audience == InsLogger.AudienceToken) {
              InsLogger.Log(
                logEvent.Level, logEvent.SourceContext, logEvent.SourceLineId, logEvent.UseCaseId,
                logEvent.MessageTemplate
              );
            }
            else {
              DevLogger.Log(
                logEvent.Level, logEvent.SourceContext, logEvent.SourceLineId, logEvent.UseCaseId,
                logEvent.MessageTemplate
              );
            }
          }
        }
      });
    }

    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private LogEventFilter _Filter = new LogEventFilter(1);

    /// <summary>
    /// Configure this filter to limit the amount of log entries which will be redirected to the remote sink.
    /// </summary>
    public LogEventFilter Filter {
      get {
        return _Filter;
      }
    }

    public LogEventFilteringRule[] GetLogLevelFilteringRules() {
      //expose the own filtering(on retrieval)-rules also to the clients
      //to avoid getting messages transferred unneeded...
      return _Filter.FilteringRulesGetter.Invoke();
    }

  }

}
