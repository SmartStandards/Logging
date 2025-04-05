using System;
using System.ComponentModel;

namespace Logging.SmartStandards.TemplateHousekeeping {

  public static class LogMessageTemplateRepository {

    internal static void GetMessageTemplateByKind(Enum kindEnumElement, out int kindId, out string messageTemplate) {

      kindId = (int)(object)kindEnumElement;

      messageTemplate = null;

      try {
        TypeConverter typeConverter = TypeDescriptor.GetConverter(kindEnumElement);
        if (typeConverter != null && typeConverter.CanConvertTo(typeof(System.String))) {
          messageTemplate = typeConverter.ConvertToString(kindEnumElement);
        }
      } catch {
      }
      if (String.IsNullOrWhiteSpace(messageTemplate)) {
        messageTemplate = Enum.GetName(kindEnumElement.GetType(), kindEnumElement);
      }
    }

  }
}
