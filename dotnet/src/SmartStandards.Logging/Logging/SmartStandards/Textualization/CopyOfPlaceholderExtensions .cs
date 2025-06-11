using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Logging.SmartStandards.Textualization {

  internal static class CopyOfPlaceholderExtensions { // v 1.0.0

    // Inner class content manually copied from:
    // https://github.com/SmartStandards/Essentials/blob/master/dotnet/src/SmartStandards.Essentials/System/SmartStandards/PlaceholderExtensions.cs

    /// <summary>
    ///   Executes a callback method for each placeholder in a template string.
    /// </summary>
    /// <param name="extendee">
    ///   A template string containing named placeholders. 
    ///   E.g. "Hello {audience}, the answer is {answer}."
    /// </param>
    /// <param name="onPlaceholderFound">
    ///   bool onPlaceholderFound(string placeholderName).
    ///   Will be called for each placeholder in order of appearance.
    ///   (e.g. "audience", "answer").
    ///   The placeholder name will be passed (without braces), unless omitPlaceholderNames is set (then null will be passed).
    ///   If the callback function returns true (= cancel), the processing will stop immedieately.
    /// </param>
    /// <param name="onRegularSegmentFound">
    ///   void onRegularSegmentFound(int pos, int length).
    ///   Optional. Will be called for each seqgment of the template that is not a placeholder.
    ///   (e.g. "Hello ", ", the answer is ", ".").
    /// </param>
    /// <param name="omitPlaceholderNames">
    ///   Performance optimization. If true, the placeholder name is not extracted from the template.
    /// </param>
    public static void ForEachPlaceholder(
      this string extendee,
      Func<string, bool> onPlaceholderFound,
      Action<int, int> onRegularSegmentFound = null,
      bool omitPlaceholderNames = false
    ) {

      if (extendee is null || extendee.Length < 3) return;

      int cursor = 0;

      do {

        int leftPos = extendee.IndexOf("{", cursor);

        if (leftPos < 0) break;

        int rightPos = extendee.IndexOf("}", cursor);

        if (rightPos < 0 || rightPos < leftPos + 1) return;

        string placeholderName = null;

        if (!omitPlaceholderNames) placeholderName = extendee.Substring(leftPos + 1, rightPos - leftPos - 1);

        onRegularSegmentFound?.Invoke(cursor, leftPos - cursor);

        if (onPlaceholderFound.Invoke(placeholderName)) return;

        cursor = rightPos + 1;

      } while (cursor < extendee.Length);

      onRegularSegmentFound?.Invoke(cursor, extendee.Length - cursor);
    }

    /// <summary>
    ///   Resolves named placeholders in a template string from arguments.
    /// </summary>
    /// <param name="extendee">
    ///   A template string containing named placeholders. 
    ///   E.g. "Hello {audience}, the answer is {answer}."
    /// </param>
    /// <param name="args">
    ///   Arguments containing the placeholder values in order of appearance in the template. Example:
    ///   "World", 42
    /// </param>
    /// <returns>
    ///   Null or a new string instance with resolved placeholders. The example would be resolved to:
    ///   "Hello World, the answer is 42."
    /// </returns>
    public static string ResolvePlaceholders(this string extendee, params object[] args) {

      int maxIndex = args != null ? args.GetUpperBound(0) : -1;

      if (extendee is null || extendee.Length < 3 || maxIndex < 0) return extendee;

      StringBuilder targetStringBuilder = new StringBuilder(extendee.Length * 15 / 10);

      targetStringBuilder.AppendResolved(extendee, args);

      return targetStringBuilder.ToString();
    }

    /// <summary>
    ///   Resolves placeholders within a StringBuilder instance.
    /// </summary>
    /// <param name="extendee"> The StringBuilder instance containing unresolved placeholders. </param>
    /// <param name="args"> Placeholder values in correct order. </param>
    /// <returns> The StringBuilder instance after resolvingvar (to support fluent syntax). </returns>
    /// <remarks>
    ///   The internal behavior of this method is NOT equivalent to the same named string extension.
    ///   Is is NOT performant to convert a string to a StringBuilder and pass it to this extension.
    ///   Only use this extension if you have a StringBuilder instance anyways and you want to keep the instance.
    ///   Otherwise using the string extension is faster.
    /// </remarks>
    public static StringBuilder ResolvePlaceholders(this StringBuilder extendee, params object[] args) {

      if (extendee == null || args == null) { return extendee; }

      if (args.Length == 0) { return extendee; }

      int cursor = 0;

      foreach (object boxedValue in args) {

        int left = -1;

        for (int i = cursor; i < extendee.Length; i++) {
          if (extendee[i] == '{') { left = i; break; }
          ;
        }

        if (left == -1) { break; }

        int right = -1;

        for (int i = left; i < extendee.Length; i++) {
          if (extendee[i] == '}') { right = i; break; }
          ;
        }

        if (right == -1) { break; }

        extendee.Remove(left, right - left + 1);

        string value = boxedValue.ToString();

        extendee.Insert(left, value);

        cursor += value.Length;
      }

      return extendee;
    }

    public static string ResolvePlaceholdersByDictionary(this string extendee, IDictionary<string, object> placeholders) {

      if (extendee is null || extendee.Length < 3 || placeholders is null || placeholders.Count == 0) {
        return extendee;
      }

      object onResolvePlaceholder(string placeholderName) {
        object value = null;
        if (placeholders.TryGetValue(placeholderName, out value)) {
          return value ?? ""; // Value is null => render empty string
        } else {
          return null; // Value not existing => return null => leave placeholder unchanged
        }
      }

      StringBuilder targetStringBuilder = new StringBuilder(extendee.Length * 15 / 10);

      targetStringBuilder.AppendResolving(extendee, onResolvePlaceholder);

      return targetStringBuilder.ToString();
    }

    public static string ResolvePlaceholdersByPropertyBag(this string extendee, object propertyBag) {

      if (extendee is null || extendee.Length < 3 || propertyBag is null) return extendee;

      object onResolvePlaceholder(string placeholderName) {

        PropertyInfo propertyInfo = propertyBag.GetType().GetProperty(
          placeholderName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance
        );

        if (propertyInfo != null) {
          return propertyInfo.GetValue(propertyBag) ?? ""; // Property value is null => render empty string
        } else {
          return null; // Property not existing => return null => leave placeholder unchanged
        }
      }

      StringBuilder targetStringBuilder = new StringBuilder(extendee.Length * 15 / 10);

      targetStringBuilder.AppendResolving(extendee, onResolvePlaceholder);

      return targetStringBuilder.ToString();
    }

    /// <summary>
    ///   Appends a resolved template string to an existing StringBuilder while calling back a resolving method for each placeholder.
    /// </summary>
    /// <param name="template"> A template string containing named placeholders. E.g. "Hello {audience}, the answer is {answer}."</param>
    /// <param name="onResolvePlaceholder">
    ///   string onResolvePlaceholder(name).
    ///   Will be called for each placeholder in order of appearance.
    ///   (e.g. "audience", "answer").
    ///   The placeholder name will be passed (or null, if omitPlaceholderNames is set).
    ///   The resolved placeholder value should be returned 
    ///   (Note: if the template contains a format-string siffix like ':yyyy-MM-dd', it is important, that the resolved placeholder value was not already converted into a string during resolving!). 
    ///   If null is returned, the placeholder will remain unchanged (including braces).
    ///   </param>
    /// <param name="omitPlaceholderNames">
    ///   Performance optimization. If true, the placeholder name is not extracted from the template.
    /// </param>
    /// <returns> The resolved string. </returns>
    public static StringBuilder AppendResolving(
      this StringBuilder extendee,
      string template,
      Func<string, object> onResolvePlaceholder,
      bool omitPlaceholderNames = false
    ) {

      if (extendee == null) return null;

      if (template is null || template.Length < 3 || onResolvePlaceholder is null) {
        extendee.Append(template);
        return extendee;
      }

      bool onPlaceholderFound(string placeholderNameOrExpression) {

        string valueAsString = null;

        if(placeholderNameOrExpression != null) {
          int formatStringSeparatorIndex = placeholderNameOrExpression.IndexOf(':');
          if (formatStringSeparatorIndex < 0) {
            valueAsString = onResolvePlaceholder.Invoke(placeholderNameOrExpression)?.ToString();
          }
          else {
            object value = onResolvePlaceholder.Invoke(placeholderNameOrExpression.Substring(0, formatStringSeparatorIndex));
            if(value != null) {
              string formatString = placeholderNameOrExpression.Substring(formatStringSeparatorIndex + 1);
              valueAsString = string.Format("{0:" + formatString + "}", value);
            }
          }
        }
        else {
          valueAsString = onResolvePlaceholder.Invoke(null)?.ToString();
        }

        if (valueAsString != null) {
          extendee.Append(valueAsString);
        } else {
          extendee.Append('{').Append(placeholderNameOrExpression).Append('}');
        }
        return false;
      }

      void onRegularSegmentFound(int pos, int length) => extendee.Append(template, pos, length);

      template.ForEachPlaceholder(onPlaceholderFound, onRegularSegmentFound, omitPlaceholderNames);

      return extendee;
    }

    public static StringBuilder AppendResolved(this StringBuilder extendee, string template, params object[] args) {

      if (extendee == null) return null;

      int maxIndex = args != null ? args.GetUpperBound(0) : -1;

      if (template is null || template.Length < 3 || maxIndex < 0) {
        extendee.Append(template);
        return extendee;
      }

      int i = -1;

      object onResolvePlaceholder(string dummyName) {
        i++;
        if (i <= maxIndex) {
          return args[i];
        } else {
          return null;
        }
      }

      extendee.AppendResolving(template, onResolvePlaceholder, true);

      return extendee;
    }

  }
}
