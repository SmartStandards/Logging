using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Logging.SmartStandards.Centralized;
using Logging.SmartStandards.Textualization;

namespace Logging.SmartStandards.Sinks {

  public class CheapFileSink : BufferedSink {

    private string _TargetFileName;
    private object _FileFlushLock = new object();

    /// <summary>
    /// </summary>
    /// <param name="targetFileName">
    /// can contain placeholders: {timestamp:yyyy-mm-dd} {audience} {sourcecontext}
    /// </param>
    /// <param name="minLogLevel">
    ///   LogLevel-Filter (min level before en event gets written into the file):
    ///   0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical (default: 2)
    /// </param>
    public CheapFileSink(string targetFileName, int minLogLevel) {
      _TargetFileName = targetFileName.Replace("{timestamp", "{0").Replace("{audience", "{1").Replace("{sourcecontext", "{2");
    }

    protected override void Flush(LogEvent[] bufferedEvents) {

      lock (_FileFlushLock) {

        var streamsPerFileName = new Dictionary<string, FileStream>();

        foreach (LogEvent bufferedEvent in bufferedEvents) {

          //build the filename by resolving the placeholders
          string targetFileName = string.Format(
            _TargetFileName,
            bufferedEvent.TimestampUtc.ToLocalTime(), bufferedEvent.Audience, bufferedEvent.SourceContext
          );

          //resolve unrooted-paths (once)
          if (!Path.IsPathRooted(targetFileName)) {
            int oldLength = targetFileName.Length;
            targetFileName = Path.GetFullPath(targetFileName);
            string appendedRootPart = targetFileName.Substring(0, targetFileName.Length - oldLength);
            _TargetFileName = appendedRootPart + _TargetFileName;
          }

          //create stream(s) on-demand and keep them open for the whole flush-operation
          FileStream targetStream = null;
          if (!streamsPerFileName.TryGetValue(targetFileName, out targetStream)) {
            const int maxRetries = 5;
            for (int attempt = 0; attempt < maxRetries; attempt++) {
              try {
                targetStream = new FileStream(targetFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, bufferSize: 4096, useAsync: false);
              } catch (IOException) {
                Thread.Sleep(50);
                if (attempt > maxRetries) {
                  throw;
                }
              }
            }
            streamsPerFileName.Add(targetFileName, targetStream);
          }

          //preapre/format the log-line
          StringBuilder logParaphBuilder = new StringBuilder(bufferedEvent.MessageTemplate.Length + 20);
          logParaphBuilder.AppendFormat("{0:yyyy-MM-dd} | {0:HH:mm:dd} | ", bufferedEvent.TimestampUtc.ToLocalTime());
          LogParaphRenderer.BuildParaphResolved(
            logParaphBuilder,
            bufferedEvent.Audience, bufferedEvent.Level, bufferedEvent.SourceContext, //TODO: hir fehken die args
            bufferedEvent.SourceLineId, bufferedEvent.EventKindId, bufferedEvent.MessageTemplate, Array.Empty<object>()
          );
          logParaphBuilder.AppendLine();

          //write the log-line into the stream
          byte[] bytes = Encoding.UTF8.GetBytes(logParaphBuilder.ToString());
          targetStream.Write(bytes, 0, bytes.Length);

        }

        //after all lines have been writen into the filestream(s), flush everything at the end...
        foreach (FileStream stream in streamsPerFileName.Values) {
          stream.Flush(true);
          stream.Dispose();
        }

      }

    }

  }

}
