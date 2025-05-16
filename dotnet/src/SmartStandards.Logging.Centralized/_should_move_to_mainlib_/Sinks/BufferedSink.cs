using Logging.SmartStandards.Centralized;
using Logging.SmartStandards.Filtering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.SmartStandards.Sinks {

  public abstract class BufferedSink : IDisposable {

    private List<LogEvent> _MessageBuffer = new List<LogEvent>();

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
          if (_MessageBuffer.Count > this.AutoFlushMaxCount) {
            break;
          }
        }
        ;
        Thread.Sleep(500);
      }
      this.Flush();
    }

    /// <summary>
    /// Flush the buffer (synchonously within this method)
    /// </summary>
    public void Flush() {
      LogEvent[] snapshot;
      lock (_MessageBuffer) {
        snapshot = _MessageBuffer.ToArray();
        _MessageBuffer.Clear();
        _AutoFlushTask = null;
      }
      try {
        this.Flush(snapshot);
      } catch {
        lock (_MessageBuffer) {
          //put the current items back into the buffer to preserve them
          _MessageBuffer.InsertRange(0, snapshot);
        }
        throw;
      }

    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private LogEventFilter _Filter = new LogEventFilter(2);

    /// <summary>
    /// Configure this filter to limit the amount of log entries which will be redirected to the remote sink.
    /// </summary>
    public LogEventFilter Filter {
      get {
        return _Filter;
      }
    }

    protected abstract void Flush(LogEvent[] bufferedEvents);

    public virtual void WriteMessage(
      string audience, int level, string sourceContext, long sourceLineId,
      int useCaseId, string messageTemplate, object[] args
    ) {

      var evt = new LogEvent {
        Audience = audience,
        Level = level,
        SourceContext = sourceContext,
        SourceLineId = sourceLineId,
        UseCaseId = useCaseId,
        MessageTemplate = messageTemplate,
        TimestampUtc = DateTime.UtcNow
        //TODO: hier fehelen noch die args?
      };

      if (!_Filter.IsMatch(evt)) {
        return;
      }

      lock (_MessageBuffer) {

        _MessageBuffer.Add(evt);

        if (_AutoFlushTask == null) {
          _AutoFlushTask = Task.Run(this.AutoFlushAwaiter);
        }

      }
    }

    public virtual void WriteException(
      string audience, int level, string sourceContext, long sourceLineId,
      int useCaseId, Exception ex
    ) {

      var evt = new LogEvent {
        Audience = audience,
        Level = level,
        SourceContext = sourceContext,
        SourceLineId = sourceLineId,
        UseCaseId = useCaseId,
        MessageTemplate = ExceptionRenderer.Render(ex, false),
        TimestampUtc = DateTime.UtcNow,
        Exception = ExceptionRenderer.Render(ex, true),
      };

      if (!_Filter.IsMatch(evt)) {
        return;
      }

      lock (_MessageBuffer) {

        _MessageBuffer.Add(evt);

        if (_AutoFlushTask == null) {
          _AutoFlushTask = Task.Run(this.AutoFlushAwaiter);
        }

      }
    }

    public void Dispose() {
      try {
        this.Flush();
      } catch { }
    }

  }

}
