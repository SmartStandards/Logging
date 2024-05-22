using System.Text;

namespace System.SmartStandards {

  internal static class CopyOfPlaceholderExtensions {

    /// <remarks>
    ///   v 0.1.0
    /// </remarks>
    public static StringBuilder ResolvePlaceholders(this StringBuilder extendee, params object[] args) {

      if (extendee == null || args == null) { return extendee; }

      if (args.Length == 0) { return extendee; }

      int cursor = 0;

      foreach (object boxedValue in args) {

        int left = -1;

        for (int i = cursor; i < extendee.Length; i++) {
          if (extendee[i] == '{') { left = i; break; };
        }

        if (left == -1) { break; }

        int right = -1;

        for (int i = left; i < extendee.Length; i++) {
          if (extendee[i] == '}') { right = i; break; };
        }

        if (right == -1) { break; }

        extendee.Remove(left, right - left + 1);

        string value = boxedValue.ToString();

        extendee.Insert(left, value);

        cursor += value.Length;
      }

      return extendee;
    }

  }
}
