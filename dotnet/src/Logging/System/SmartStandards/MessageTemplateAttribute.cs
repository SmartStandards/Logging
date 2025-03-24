namespace System.SmartStandards {

  /// <summary>
  ///   This Attribute was made to be placed over each individual field of an enum which 
  ///   also has the 'System.SmartStandards.EnumMessageTypeConverter'. It declares
  ///   a 'messageTemplate' string assiged to the corresponding enum field
  ///   (which is representing a wellknown message) in order to be used when
  ///   invoking Log methods on the SmartStandards 'DevLogger', 'InfrastructureLogger' or
  ///   'ProtocolLogger'
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
  public class MessageTemplateAttribute : Attribute {

    /// <summary>
    ///   This Attribute was made to be placed over each individual field of an enum which 
    ///   also has the 'System.SmartStandards.EnumMessageTypeConverter'. It declares
    ///   a 'messageTemplate' string assiged to the corresponding enum field
    ///   (which is representing a wellknown message) in order to be used when
    ///   invoking Log methods on the SmartStandards 'DevLogger', 'InfrastructureLogger' or
    ///   'ProtocolLogger'
    /// </summary>
    /// <param name="messageTemplate"></param>
    /// <param name="language"> ISO code like 'en-us' or 'de-de' </param>
    public MessageTemplateAttribute(string messageTemplate, string language = null) {
      this.MessageTemplate = messageTemplate;
      this.Language = language;
    }

    public string MessageTemplate { get; }

    public string Language { get; }

  }

}
