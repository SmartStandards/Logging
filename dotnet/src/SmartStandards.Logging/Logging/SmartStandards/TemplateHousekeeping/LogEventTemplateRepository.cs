using System;
using System.ComponentModel;

namespace Logging.SmartStandards.TemplateHousekeeping {

  public static class LogEventTemplateRepository { // v 1.0.0

    internal static void GetLogEventTemplateByEnum(Enum templateEnumElement, out int kindId, out string messageTemplate) {

      kindId = (int)(object)templateEnumElement;

      messageTemplate = null;

      try {
        TypeConverter typeConverter = TypeDescriptor.GetConverter(templateEnumElement);
        if (typeConverter != null && typeConverter.CanConvertTo(typeof(System.String))) {
          messageTemplate = typeConverter.ConvertToString(templateEnumElement);
        }
      } catch {
      }
      if (String.IsNullOrWhiteSpace(messageTemplate)) {
        messageTemplate = Enum.GetName(templateEnumElement.GetType(), templateEnumElement);
      }
    }

  }
}
