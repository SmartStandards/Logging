using System;
using System.ComponentModel;

namespace Logging.SmartStandards {

  public static class LoggingHelper {

    internal static void GetLogTemplateFromEnum(Enum logTemplate, out int id, out string messageTemplate) {
      id = (int)(object)logTemplate;
      messageTemplate = null;
      try {
        TypeConverter typeConverter = TypeDescriptor.GetConverter(logTemplate);
        if (typeConverter != null && typeConverter.CanConvertTo(typeof(System.String))) {
          messageTemplate = typeConverter.ConvertToString(logTemplate);
        }
      } catch {
      }
      if (String.IsNullOrWhiteSpace(messageTemplate)) {
        messageTemplate = Enum.GetName(logTemplate.GetType(), logTemplate);
      }
    }

  }
}
