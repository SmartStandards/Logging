using System;

namespace Logging.SmartStandards {

  /// <summary>
  ///   This Attribute was made to be placed over each individual field of an enum which 
  ///   also has the 'System.SmartStandards.EnumMessageTypeConverter'. It declares
  ///   a 'messageTemplate' string assiged to the corresponding enum field
  ///   (which is representing a wellknown message) in order to be used when
  ///   invoking Log methods on the SmartStandards 'DevLogger', 'InfrastructureLogger' or
  ///   'ProtocolLogger'
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
  public class LogMessageTemplateAttribute : Attribute {

    /// <summary>
    ///   This Attribute was made to be placed over each individual field of an enum which 
    ///   also has the 'System.SmartStandards.EnumMessageTypeConverter'. It declares
    ///   a 'messageTemplate' string assiged to the corresponding enum field
    ///   (which is representing a wellknown message) in order to be used when
    ///   invoking Log methods on the SmartStandards 'DevLogger', 'InfrastructureLogger' or
    ///   'ProtocolLogger'
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
