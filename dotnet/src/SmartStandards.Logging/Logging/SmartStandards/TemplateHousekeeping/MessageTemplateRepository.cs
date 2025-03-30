using System;
using System.ComponentModel;

namespace Logging.SmartStandards.TemplateHousekeeping {

  public static class MessageTemplateRepository {

    internal static void GetMessageTemplateByEnum(Enum enumBasedMessageTemplate, out int eventId, out string messageTemplate) {

      eventId = (int)(object)enumBasedMessageTemplate;

      messageTemplate = null;

      try {
        TypeConverter typeConverter = TypeDescriptor.GetConverter(enumBasedMessageTemplate);
        if (typeConverter != null && typeConverter.CanConvertTo(typeof(System.String))) {
          messageTemplate = typeConverter.ConvertToString(enumBasedMessageTemplate);
        }
      } catch {
      }
      if (String.IsNullOrWhiteSpace(messageTemplate)) {
        messageTemplate = Enum.GetName(enumBasedMessageTemplate.GetType(), enumBasedMessageTemplate);
      }
    }

  }
}
