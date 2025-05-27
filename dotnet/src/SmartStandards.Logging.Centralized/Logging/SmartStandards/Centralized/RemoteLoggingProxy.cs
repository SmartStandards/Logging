using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Logging.SmartStandards.Filtering;
using Logging.SmartStandards.Transport;

namespace Logging.SmartStandards.Centralized {

  /// <summary>
  /// An (Server-Side) implementation of ILogEntrySink which easily can
  /// be exposed as service-endpoint to receive incomming LogEntries
  /// (usually comming from the 'LogEntryRemoteSink' at Client-Side)
  /// and forward them into the SmartStandars-Logger on server-side!
  /// NOTE: during transition from LogEvent to LogEntry, any Exception-typed events were already "rendered"
  /// to a Exception-string (which is more a "Message" than an "Exception"),
  /// when re-emitting we'll need to accept this small information-loss
  /// (never the less all other CustomFields will get lost, because were going one layer down again).
  /// </summary>
  public class RemoteLoggingProxy : ILogEntrySink {

    public void ReceiveLogEntries(LogEntry[] logEntries) {
      Task.Run((Action)(() => {
        foreach (LogEntry logEntry in logEntries) {

          if (logEntry.Audience == BizLogger.AudienceToken) {
            if (Routing.BizLoggerToTraceBus) {
              logEntry.ReEmitTo(Routing.InternalTraceBusFeed.EmitMessage);
            }
            if (Routing.BizLoggerToCustomBus) {
              logEntry.ReEmitTo(CustomBusFeed.OnEmitMessage);
            }
          }
          else if (logEntry.Audience == InsLogger.AudienceToken) {
            if (Routing.InsLoggerToTraceBus) {
              logEntry.ReEmitTo(Routing.InternalTraceBusFeed.EmitMessage);
            }
            if (Routing.InsLoggerToCustomBus) {
              logEntry.ReEmitTo(CustomBusFeed.OnEmitMessage);
            }
          }
          else { // (logEntry.Audience == DevLogger.AudienceToken)
            if (Routing.DevLoggerToTraceBus) {
              logEntry.ReEmitTo(Routing.InternalTraceBusFeed.EmitMessage);
            }
            if (Routing.DevLoggerToCustomBus) {
              logEntry.ReEmitTo(CustomBusFeed.OnEmitMessage);
            }
          }
 
        }//loop
      }));
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private LogEntryFilter _Filter = new LogEntryFilter(1);

    /// <summary>
    /// Configure this filter to limit the amount of log entries which will be redirected to the remote sink.
    /// </summary>
    public LogEntryFilter Filter {
      get {
        return _Filter;
      }
    }

    public LogEntryFilteringRule[] GetLogLevelFilteringRules() {
      //expose the own filtering(on retrieval)-rules also to the clients
      //to avoid getting messages transferred unneeded...
      return _Filter.FilteringRulesGetter.Invoke();
    }

  }

}
