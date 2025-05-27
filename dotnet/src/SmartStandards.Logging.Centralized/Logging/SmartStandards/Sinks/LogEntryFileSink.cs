using Logging.SmartStandards.Centralized;
using Logging.SmartStandards.Filtering;
using Logging.SmartStandards.Textualization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.SmartStandards;
using System.Text;
using System.Threading;

namespace Logging.SmartStandards.Sinks {

  public class LogEntryFileSink : ILogEntrySink {

    private string _TargetFileName;
    private object _FileFlushLock = new object();

    /// <summary>
    /// </summary>
    /// <param name="targetFileName">
    /// can contain placeholders: {timestamp:yyyy-mm-dd} {audience} {sourcecontext}
    /// </param>
    public LogEntryFileSink(string targetFileName) {
      _TargetFileName = targetFileName;
    }

    public void ReceiveLogEntries(LogEntry[] logEntries) {

      lock (_FileFlushLock) {

        var streamsPerFileName = new Dictionary<string, FileStream>();

        try {

          foreach (LogEntry logEntry in logEntries) {

            string targetFileName = GetTargetLogFileNameForEntry(logEntry);

            //create stream(s) on-demand and keep them open for the whole flush-operation
            FileStream targetStream = null;
            if(!streamsPerFileName.TryGetValue(targetFileName, out targetStream)) {
              const int maxRetries = 5;
              for (int attempt = 0; attempt < maxRetries; attempt++) {
                try { 
                  targetStream = new FileStream(targetFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, bufferSize: 4096, useAsync: false);
                }
                catch (IOException)
                {
                  Thread.Sleep(50);
                  if(attempt > maxRetries) {
                    throw;
                  }
                }
              }       
              streamsPerFileName.Add(targetFileName, targetStream);
            }

            //preapre/format the log-line
            StringBuilder logParaphBuilder = new StringBuilder(logEntry.MessageTemplate.Length + 20);
            logParaphBuilder.AppendFormat("{0:yyyy-MM-dd} | {0:HH:mm:dd} | ", logEntry.GetTimestampAsDateTime());
            LogParaphRenderer.BuildParaphResolved(
              logParaphBuilder,
              logEntry.Audience, logEntry.Level, logEntry.SourceContext, //TODO: hir fehken die args
              logEntry.SourceLineId, logEntry.EventKindId, logEntry.MessageTemplate, Array.Empty<object>()
            );
            logParaphBuilder.AppendLine();

            //write the log-line into the stream
            byte[] bytes = Encoding.UTF8.GetBytes(logParaphBuilder.ToString());
            targetStream.Write(bytes, 0, bytes.Length);

          }

        }
        finally {

          //after all lines have been writen into the filestream(s), flush everything at the end...
          foreach (FileStream stream in streamsPerFileName.Values) {
            stream.Flush(true);
            stream.Dispose();
          }

        }

      }

    }

    private string GetTargetLogFileNameForEntry(LogEntry entry) {

      StringBuilder targetFileNameBuilder = new StringBuilder(_TargetFileName.Length + 100);
      targetFileNameBuilder.AppendResolving(
        _TargetFileName,
        (placeholderExpression) => ResolveFilenamePlaceholder(placeholderExpression, entry)
      );
      string targetFileName = targetFileNameBuilder.ToString();

      //resolve unrooted-paths (once)
      if (!Path.IsPathRooted(targetFileName)) {
        int oldLength = targetFileName.Length;
        targetFileName = Path.GetFullPath(targetFileName);
        string appendedRootPart = targetFileName.Substring(0, targetFileName.Length - oldLength);
        _TargetFileName = appendedRootPart + _TargetFileName;
      }

      return targetFileName;
    }

    private static object ResolveFilenamePlaceholder(string placeholderExpression, LogEntry entry) {
      if (placeholderExpression.Equals(nameof(entry.Timestamp), StringComparison.CurrentCultureIgnoreCase)) {
        entry.GetTimestampAsDateTime();
      }
      if (placeholderExpression.Equals(nameof(entry.Application), StringComparison.CurrentCultureIgnoreCase)) {
        return entry.Application ?? "";
      }
      if (placeholderExpression.Equals(nameof(entry.Audience), StringComparison.CurrentCultureIgnoreCase)) {
        return entry.Audience ?? "";
      }
      if (placeholderExpression.Equals(nameof(entry.SourceContext), StringComparison.CurrentCultureIgnoreCase)) {
        return entry.SourceContext ?? "";
      }
      return string.Empty;
    }

    #region " Filter-Rules "

    private List<LogEntryFilteringRule> _FilteringRules = new List<LogEntryFilteringRule>();

    public IList<LogEntryFilteringRule> FilteringRules {
      get {
        return new List<LogEntryFilteringRule>();
      }
    }

    LogEntryFilteringRule[] ILogEntrySink.GetLogLevelFilteringRules() {
      lock (_FilteringRules) {
        return _FilteringRules.ToArray();
      }
    }

    #endregion

  }

}
