using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Logging.SmartStandards.TemplateHousekeeping {

  public class LogMessageEnumConverter : EnumConverter {

    private Array _FlagValues;

    private bool _IsFlagEnum = false;

    private Dictionary<CultureInfo, Dictionary<string, object>> _CachesPerLanguage = new Dictionary<CultureInfo, Dictionary<string, object>>();

    public LogMessageEnumConverter(Type enumType) : base(enumType) {
      if (enumType.GetCustomAttributes(typeof(FlagsAttribute), true).Any()) {
        _IsFlagEnum = true;
        _FlagValues = Enum.GetValues(enumType);
      }
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {

      if (value is string) {
        object result;

        if (_IsFlagEnum) {
          result = GetMessageTemplateFromAttributeByEnumFlag(culture, (string)value);
        } else {
          result = GetMessageTemplateFromCache(culture, (string)value);
        }

        if (result == null) {
          if (value != null) {
            result = base.ConvertFrom(context, culture, value);
          }
        }

        return result;
      }

      return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

      // at system.xaml.dll

      if (context == null || !(context.GetType().FullName.Equals("System.Windows.Markup.IValueSerializerContext"))) {
        if ((value != null) && (destinationType.Equals(typeof(System.String)))) {
          object result;
          if ((_IsFlagEnum)) {
            result = GetMessageTemplateFromAttributeByEnumFlagValue(culture, value);
          } else {
            result = GetMessageTemplateFromAttributeByEnumValue(culture, value);
          }
          return result;
        }
      }

      return base.ConvertTo(context, culture, value, destinationType);
    }

    public static string ConvertToString(Enum value) {
      if (value != null) {
        TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
        return converter.ConvertToString(value);
      }
      return string.Empty;
    }

    private object GetMessageTemplateFromAttributeByEnumFlag(CultureInfo culture, string text) {
      Dictionary<string, object> languageSpecificCache = GetLanguageSpecificCache(culture);
      string[] textValues = text.Split(',');
      ulong result = 0;

      foreach (string textValue in textValues) {
        object value = null;
        string trimmedTextValue = textValue.Trim();

        if ((!languageSpecificCache.TryGetValue(trimmedTextValue, out value)))
          return null;

        result = result | Convert.ToUInt32(value);
      }

      return Enum.ToObject(this.EnumType, result);
    }

    private Dictionary<string, object> GetLanguageSpecificCache(CultureInfo culture) {
      lock (_CachesPerLanguage) {
        Dictionary<string, object> result = null;
        if (culture == null) {
          culture = CultureInfo.CurrentCulture;
        }
        if (!_CachesPerLanguage.TryGetValue(culture, out result)) {
          result = new Dictionary<string, object>();
          foreach (var value in this.GetStandardValues()) {
            var text = this.GetMessageTemplateFromAttributeByEnumValue(culture, value);
            if (text != null) {
              result.Add(text, value);
            }
          }
          _CachesPerLanguage.Add(culture, result);
        }
        return result;
      }
    }

    private object GetMessageTemplateFromCache(CultureInfo culture, string text) {
      Dictionary<string, object> languageSpecificCache = this.GetLanguageSpecificCache(culture);
      object result = null;
      languageSpecificCache.TryGetValue(text, out result);
      return result;
    }

    private string GetMessageTemplateFromAttributeByEnumValue(CultureInfo culture, object value) {

      if (value == null) {
        return string.Empty;
      }

      Type type = value.GetType();

      if (!type.IsEnum) {
        return value.ToString();
      }

      LogMessageTemplateAttribute[] attributes = GetEnumFieldAttributes<LogMessageTemplateAttribute>((Enum)value);

      LogMessageTemplateAttribute defaultAttribute = attributes.FirstOrDefault();

      foreach (LogMessageTemplateAttribute attribute in attributes) {
        if (string.IsNullOrEmpty(attribute.Language)) {
          defaultAttribute = attribute;
        } else if (attribute.Language.Equals(culture.Name, StringComparison.InvariantCultureIgnoreCase)) {
          return attribute.LogMessageTemplate;
        }
      }
      if (defaultAttribute != null) {
        return defaultAttribute.LogMessageTemplate;
      } else {
        return Enum.GetName(type, value);
      }
    }

    private string GetMessageTemplateFromAttributeByEnumFlagValue(CultureInfo culture, object value) {
      if (Enum.IsDefined(value.GetType(), value)) {
        return this.GetMessageTemplateFromAttributeByEnumValue(culture, value);
      }
      long lValue = Convert.ToInt32(value);
      string result = null;
      foreach (object flagValue in _FlagValues) {
        long lFlagValue = Convert.ToInt32(flagValue);
        if (this.CheckSingleBit(lFlagValue)) {
          if ((lFlagValue & lValue) == lFlagValue) {
            string valueText = this.GetMessageTemplateFromAttributeByEnumValue(culture, flagValue);
            if (result == null) {
              result = valueText;
            } else {
              result = string.Format("{0}+{1}", result, valueText);
            }
          }
        }
      }

      return result;
    }

    public static List<KeyValuePair<Enum, string>> GetValues(Type enumType) {
      return GetValues(enumType, CultureInfo.CurrentUICulture);
    }

    public static List<KeyValuePair<Enum, string>> GetValues(Type enumType, CultureInfo culture) {
      List<KeyValuePair<Enum, string>> result = new List<KeyValuePair<Enum, string>>();
      TypeConverter converter = TypeDescriptor.GetConverter(enumType);
      foreach (System.Enum value in Enum.GetValues(enumType)) {
        KeyValuePair<Enum, string> pair = new KeyValuePair<Enum, string>(
          value, converter.ConvertToString(null, culture, value)
        );
        result.Add(pair);
      }
      return result;
    }

    private bool CheckSingleBit(long value) {
      switch (value) {
        case 0: {
          return false;
        }
        case 1: {
          return true;
        }
      }
      return ((value & (value - 1)) == 0);
    }

    private TAttribute[] GetEnumFieldAttributes<TAttribute>(Enum enumValue) where TAttribute : Attribute {
      Type enumType = enumValue.GetType();
      string enumFieldName = Enum.GetName(enumType, enumValue);
      if ((enumFieldName == null))
        return new TAttribute[] { };
      else {
        FieldInfo enumField = enumType.GetField(enumFieldName);
        return enumField.GetCustomAttributes(false).OfType<TAttribute>().ToArray();
      }
    }

  }

}
