using System;
using System.ComponentModel;

namespace Logging.SmartStandards.UseCaseManagement {

  public static class LogUseCaseRepository { // v 1.0.0

    internal static void GetUseCaseIdAndMessageTemplateByEnum(Enum useCaseEnumElement, out int useCaseId, out string messageTemplate) {

      useCaseId = (int)(object)useCaseEnumElement;

      messageTemplate = null;

      try {
        TypeConverter typeConverter = TypeDescriptor.GetConverter(useCaseEnumElement);
        if (typeConverter != null && typeConverter.CanConvertTo(typeof(System.String))) {
          messageTemplate = typeConverter.ConvertToString(useCaseEnumElement);
        }
      } catch {
      }
      if (String.IsNullOrWhiteSpace(messageTemplate)) {
        messageTemplate = Enum.GetName(useCaseEnumElement.GetType(), useCaseEnumElement);
      }
    }

  }
}
