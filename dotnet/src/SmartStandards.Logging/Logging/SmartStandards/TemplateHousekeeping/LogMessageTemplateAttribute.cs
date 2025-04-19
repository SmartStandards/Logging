using System;

namespace Logging.SmartStandards.TemplateHousekeeping {

  /// <summary>
  ///   Defines a log message template per enum value (for LogEventTemplate enums).
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
  public class LogMessageTemplateAttribute : Attribute { // v 1.0.0

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
