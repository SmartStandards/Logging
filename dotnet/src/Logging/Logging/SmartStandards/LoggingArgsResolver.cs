using System;
using System.Text;

namespace Logging.SmartStandards {

  /// <summary>
  ///   Copy of the ArgsResolver from SmartStandards.Essentials.
  /// </summary>
  /// <remarks>
  ///   This copy was made to avoid a dependency to the SmartStandards.Essentials package.
  /// </remarks>
  internal static class LoggingArgsResolver {

    public static string TryResolveArgs(this string extendee, Object[] args, int startIndex = 0) {

      if (extendee == null || args == null) { return extendee; }

      if (args.Length == 0) { return extendee; }

      StringBuilder sb = new StringBuilder(extendee, (int)(extendee.Length * 1.5));
      return LoggingArgsResolver.TryResolveArgs(sb, args, startIndex)?.ToString();
    }

    public static StringBuilder TryResolveArgs(this StringBuilder extendee, Object[] args, int startIndex = 0) {

      if (extendee == null || args == null) { return extendee; }

      if (args.Length == 0) { return extendee; }

      int cursor = startIndex;

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
