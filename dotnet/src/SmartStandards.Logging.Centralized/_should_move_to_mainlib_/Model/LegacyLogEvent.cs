using System;
using System.Collections.Generic;

namespace Logging.SmartStandards.Centralized {

  /// <summary>
  /// A LogEvent which is reduced to be compaible with the LogEvent-class of the 'Serilog'-Framework,
  /// PLEASE CONSIDER TO USE THE FULL 'LogEvent' INSTEAD!
  /// </summary>
  [Obsolete("A LogEvent which is reduced to be compaible with the LogEvent-class of the 'Serilog'-Framework, PLEASE CONSIDER TO USE THE FULL 'LogEvent' INSTEAD!")]
  public class LegacyLogEvent {


    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = "";

    public string RenderedMessage { get; set; } = "";

    public string MessageTemplate { get; set; } = "";

    public string Exception { get; set; }

    public Dictionary<string, object> Properties { get; set; }


    #region " Convenince zum Mappen auf neues LogEvent "

    public LogEvent ToLogEvent() {

      LogEvent logEvent = new LogEvent();

      switch (this.Level) {
        case "Verbose":
          logEvent.Level = 0;
          break;
        case "Debug":
          logEvent.Level = 1;
          break;
        case "Information":
          logEvent.Level = 2;
          break;
        case "Warning":
          logEvent.Level = 3;
          break;
        case "Error":
          logEvent.Level = 4;
          break;
        case "Fatal":
          logEvent.Level = 5;
          break;
      }

      logEvent.Audience = this.Properties["Channel"]?.ToString();
      logEvent.Origin = this.Properties["Origin"]?.ToString();
      logEvent.UseCaseId = Convert.ToInt32(this.Properties["EventId"]);
      logEvent.MessageTemplate = this.Properties["Message"]?.ToString();

      //TODO: noch nicht vollständig!

      return logEvent;
    }

    #endregion

  }

}
