using Logging.SmartStandards.Centralized;
using Logging.SmartStandards.Filtering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.SmartStandards {

  /// <summary>
  /// This class provides enhanced convenience for:
  /// Conversion to 'LogEntries', Enrichment (custom fields) and Buffering
  /// </summary>
  public  static partial class LogEntryCreator {

    [DebuggerDisplay("Dispatcher: {SinkName} {BufferedEntryCount}")]
    private class LogEntryDispatcher {

      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      private ILogEntrySink _Sink;

      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      private List<LogEntry> _MessageBuffer = new List<LogEntry>();

      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      private LogEntryFilter _Filter;

      public LogEntryDispatcher(ILogEntrySink sink) {
        _Sink = sink;
        _Filter = new LogEntryFilter(2, sink.GetLogLevelFilteringRules);
      }

      #region " Properties "

      /// <summary>
      /// Configure this filter to limit the amount of log entries which will be redirected to the remote sink.
      /// </summary>
      public ILogEntrySink Sink {
        get {
          return _Sink;
        }
      }

      public string SinkName {
        get {
          return _Sink.GetType().Name;
        }
      }

      /// <summary>
      /// Configure this filter to limit the amount of log entries which will be redirected to the remote sink.
      /// </summary>
      public LogEntryFilter Filter {
        get {
          return _Filter;
        }
      }

      public int BufferedEntryCount {
        get {
          lock (_MessageBuffer) {
            return _MessageBuffer.Count;
          }
        }
      }

      #endregion

      public void BufferEntry(LogEntry entry) {

        if (!_Filter.IsMatch(entry)) {
          return;
        }

        lock (_MessageBuffer) {

          _MessageBuffer.Add(entry);

          if (_AutoFlushTask == null) {
            _AutoFlushTask = Task.Run(this.AutoFlushAwaiter);
          }

        }

        if (_AutoFlushTask == null) {
          _AutoFlushTask = Task.Run(AutoFlushAwaiter);
        }

      }

      /// <summary>
      /// Flush the buffer (synchonously within this method)
      /// </summary>
      public void FlushToSink() {
        LogEntry[] entries;
        lock (_MessageBuffer) {
          entries = _MessageBuffer.ToArray();
          _MessageBuffer.Clear();
          _AutoFlushTask = null;
        }
        try {

          _Sink.ReceiveLogEntries(entries);

        }
        catch {
          lock (_MessageBuffer) {
            //put the current items back into the buffer to preserve them
            _MessageBuffer.InsertRange(0, entries);
          }
          throw;
        }
      }

      #region " Auto-Flush "

      /// <summary>
      /// Maximum timespan (in seconds) that a message will stay within the buffer
      /// (default: 10 seconds)
      /// </summary>
      public int AutoFlushMaxAge { get; set; } = 10;

      /// <summary>
      /// Maximum number of Messages within the buffer.
      /// (default: 100 messages)
      /// </summary>
      public int AutoFlushMaxCount { get; set; } = 100;

      private Task _AutoFlushTask = null;

      private void AutoFlushAwaiter() {
        DateTime waitUntil = DateTime.Now.AddSeconds(AutoFlushMaxAge);
        Thread.Sleep(500);
        while (DateTime.Now < waitUntil) {
          lock (_MessageBuffer) {
            if (_MessageBuffer.Count > AutoFlushMaxCount) {
              break;
            }
          }
          ;
          Thread.Sleep(500);
        }
        FlushToSink();
      }

      #endregion

    }

  }

}
