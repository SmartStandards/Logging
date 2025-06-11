using Logging.SmartStandards.Textualization;
using Logging.SmartStandards.Transport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.SmartStandards;

namespace Logging.SmartStandards.Centralized {

  [DebuggerDisplay("{Audience}: {MessageTemplate}|{Exception}")]
  public class LogEntry {

    /// <summary>
    /// Recommended to be a Snowflake44
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    ///   Legt die Zielgruppe fest: "DEV" für Programmierer, "INFRA" für Dev-Ops, "PROT" für Endanwender.
    ///   (siehe Enum LogChannel)
    /// </summary>
    public string Audience { get; set; } = "*";

    /// <summary>
    ///   wird mit enum Werten belegt:
    ///
    ///   0: "Trace" (entspricht bei Serilog dem Level "Verbose")
    ///   1: "Debug"
    ///   2: "Info"
    ///   3: "Warning"
    ///   4: "Error"
    ///   5: "Fatal"
    /// </summary>
    public int Level { get; set; }

    public string SourceContext { get; set; } = "*";

    public long SourceLineId { get; set; } = 0;

    public int EventKindId { get; set; } = 0;

    public string MessageTemplate { get; set; } = null;

    public Dictionary<string, string> CustomFields { get; set; } = null;

    public string Exception { get; set; } = null;

    public string Application { get; set; } = "";

    public string HostName { get; set; } = "";

  }

  public static class LogEntryExtensions {

    /// <summary>
    /// Decodes the numeric timestamp (Snowflake44) into a DateTime and returns it as LOCAL Date.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static DateTime GetTimestampAsDateTime(this LogEntry entry) {
      return entry.GetTimestampAsDateTimeUtc().ToLocalTime();
    }

    /// <summary>
    /// Decodes the numeric timestamp (Snowflake44) into a DateTime and returns it as UTC Date.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static DateTime GetTimestampAsDateTimeUtc(this LogEntry entry) {
      return Snowflake44.DecodeDateTime(entry.Timestamp);
    }

    /// <summary>
    /// NOTE (why there is only the EmitMessageDelegate param): during transition from LogEvent to LogEntry, any Exception-typed events were already "rendered"
    /// to a Exception-string (which is more a "Message" than an "Exception"),
    /// when re-emitting we'll need to accept this small information-loss
    /// (never the less all other CustomFields will get lost, because were going one layer down again).
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="messageEmittingTarget"></param>
    internal static void ReEmitTo(this LogEntry entry, CustomBusFeed.EmitMessageDelegate messageEmittingTarget) {

      string msg;
      var args = new List<object>();
      if(entry.Exception != null) {
        msg = entry.Exception;
      }
      else {
        msg = entry.MessageTemplate;
      }

      msg.ForEachPlaceholder((string name) => {
        args.Add(entry.CustomFields.TryGetValue(name, out string value) ? value : null);
        return false; //continue
      });

      if (messageEmittingTarget != null) {
        messageEmittingTarget.Invoke(
          entry.Audience,
          entry.Level,
          entry.SourceContext,
          entry.SourceLineId,
          entry.EventKindId,
          msg,
          args.ToArray()
        );
      }

    }

  }

  //public abstract class LogEvent {

  //  /// <summary>
  //  ///   Legt die Zielgruppe fest: "DEV" für Programmierer, "INFRA" für Dev-Ops, "PROT" für Endanwender.
  //  ///   (siehe Enum LogChannel)
  //  /// </summary>
  //  public string Audience { get; set; } = "*";

  //  /// <summary>
  //  ///   wird mit enum Werten belegt:
  //  ///
  //  ///   0: "Trace" (entspricht bei Serilog dem Level "Verbose")
  //  ///   1: "Debug"
  //  ///   2: "Info"
  //  ///   3: "Warning"
  //  ///   4: "Error"
  //  ///   5: "Fatal"
  //  /// </summary>
  //  public int Level { get; set; }

  //  public string SourceContext { get; set; } = "*";

  //  public long SourceLineId { get; set; } = 0;

  //  public int EventKindId { get; set; }

  //  public string MessageTemplate { get; set; }

  //  public object[] Args { get; set; } = null;

  //  public Exception Exception { get; set; } = null;

  //}

}
