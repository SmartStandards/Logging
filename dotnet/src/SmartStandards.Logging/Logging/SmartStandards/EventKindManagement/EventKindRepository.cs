using System;
using System.ComponentModel;

namespace Logging.SmartStandards.EventKindManagement {

  public static class EventKindRepository { // v 1.0.0

    internal static void GetKindIdAndMessageTemplateByEnum(Enum eventKindEnumElement, out int eventKindId, out string messageTemplate) {

      eventKindId = (int)(object)eventKindEnumElement;

      messageTemplate = null;

      try {
        TypeConverter typeConverter = TypeDescriptor.GetConverter(eventKindEnumElement);
        if (typeConverter != null && typeConverter.CanConvertTo(typeof(System.String))) {
          messageTemplate = typeConverter.ConvertToString(eventKindEnumElement);
        }
      } catch {
      }
      if (String.IsNullOrWhiteSpace(messageTemplate)) {
        messageTemplate = Enum.GetName(eventKindEnumElement.GetType(), eventKindEnumElement);
      }
    }

  }
}
