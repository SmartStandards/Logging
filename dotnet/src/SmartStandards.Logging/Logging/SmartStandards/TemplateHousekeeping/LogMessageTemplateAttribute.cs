using System;

namespace Logging.SmartStandards.TemplateHousekeeping {

  /// <summary>
  ///   Defines a log message template for an enum value (representing the log event kind id).
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
  public class LogMessageTemplateAttribute : Attribute {

    /// <summary>
    ///  Constructor.
    /// </summary>
    /// <param name="logMessageTemplate"></param>
    /// <param name="language"> ISO code like 'en-us' or 'de-de' </param>
    public LogMessageTemplateAttribute(string logMessageTemplate, string language = null) {
      this.LogMessageTemplate = logMessageTemplate;
      this.Language = language;
    }

    public string LogMessageTemplate { get; }

    public string Language { get; }

  }

}
