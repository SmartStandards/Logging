using System;
using System.Collections.Generic;

namespace Logging.SmartStandards.Centralized {

  public class LogEvent {


    //TODO: diskussion datenhaltung: soll hier am model ein (ggf. natürlicher?)
    //      key zur addressierung eines logeintrags sein?

    /// <summary>
    ///   auto increment on db
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///   Der Zeitstempel zu dem Zeitpunkt, zu dem das Event stattgefunden hat 
    /// </summary>
    public DateTime TimestampUtc { get; set; }







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

    public int UseCaseId { get; set; }




    public string MessageTemplate { get; set; }
    //TODO: wio sind dann die Args? in den CustomFields??

    //TODO: abgrenzug zu MessageTemplate dokumentieren? is ide hier serialisiert?
    public string Exception { get; set; }


    //TODO: ist das nicht die applkation?
    public string Origin { get; set; }






    //TODO: es folgen properitre felder - wollwin wir die oder nicjt?

    public string ServerName { get; set; } = "";

    // identity state manager
    public string RuntimeUserWindowsLogonName { get; set; } = "";

    // identity state manager: InteractingUserLogonName?
    public string InteractingUserWindowsLogonName { get; set; } = "";

    // calltree anchor
    public Guid CallTreeNodeId { get; set; }




    //TODO: geht das so übern drahrt oder wollen wir nicht lieber string,string erzwingen
    public Dictionary<string, object> CustomFields { get; set; } = null;


  }

}
