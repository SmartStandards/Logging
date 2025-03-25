using Logging.SmartStandards.Textualization;
using System;
using System.Text;

namespace Logging.SmartStandards.Sinks {

  public class ConsoleSink {

    public static void WriteLogEvent(
      string audience, int level, string sourceContext, long sourceLineId,
      int eventId, string messageTemplate, object[] args
    ) {

      var rescuedColor = Console.ForegroundColor;

      StringBuilder logParaphBuilder = new StringBuilder(messageTemplate.Length + 20);

      LogParaphRenderer.BuildParaphLeftPart(logParaphBuilder, level, sourceContext, eventId);

      LogParaphRenderer.BuildParaphRightPart(logParaphBuilder, sourceLineId, audience, messageTemplate);

      switch (level) {

        case 5: { // Critical => StdErr
          Console.ForegroundColor = ConsoleColor.Magenta;
          Console.Error.WriteLine(logParaphBuilder.ToString());
          break;
        }

        case 4: { // Error => StdErr
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Error.WriteLine(logParaphBuilder.ToString());
          break;
        }

        case 3: { // Warning => StdOut
          Console.ForegroundColor = ConsoleColor.DarkYellow;
          Console.WriteLine(logParaphBuilder.ToString()); // Warnings go through StdOut, because they usually don't abort higher level actions
          break;
        }

        case 2: { // Information => StdOut
          Console.WriteLine(logParaphBuilder.ToString());
          break;
        }

        case 1: { // Debug => StdOut
          Console.ForegroundColor = ConsoleColor.DarkCyan;
          Console.WriteLine(logParaphBuilder.ToString());
          break;
        }

        default: { // Trace => StdOut
          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.WriteLine(logParaphBuilder.ToString());
          break;
        }

      }

      Console.ForegroundColor = rescuedColor;

    }
  }
}
