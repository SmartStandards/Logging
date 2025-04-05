using Logging.SmartStandards.Textualization;
using System;
using System.Text;

namespace Logging.SmartStandards.Sinks {

  public class ConsoleSink {

    public static void WriteMessage(
      string audienceToken, int level, string sourceContext, long sourceLineId,
      int kindId, string messageTemplate, object[] args
    ) {

      var rescuedColor = Console.ForegroundColor;

      StringBuilder logParaphBuilder = new StringBuilder(messageTemplate.Length + 20);

      LogParaphRenderer.BuildParaphResolved(logParaphBuilder, audienceToken, level, sourceContext, sourceLineId, kindId, messageTemplate, args);

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

    public void WriteException(
      string audience, int level, string sourceContext, long sourceLineId,
      int kindId, Exception ex
    ) {
      string exAsString = ExceptionRenderer.Render(ex);
      ConsoleSink.WriteMessage(audience, level, sourceContext, sourceLineId, kindId, exAsString, null);
    }

  }
}
