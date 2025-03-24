using System;

namespace Logging.SmartStandards.Sinks {

  public class ConsoleSink {

    public static void WriteLogEvent(
      string audience, int level, string sourceContext, long sourceLineId,
      int eventId, string messageTemplate, object[] args
    ) {

      var rescuedColor = Console.ForegroundColor;

      // [LevelAsAlpha3] SourceContext #EventId#  SourceLineId [AudienceToken]: MessageTemplate 
      // [Err] MyApp.exe #4711# 2070198253252296432 [Ins]: File not found on Disk! 

      switch (level) {

        case 5: { // Critical
          Console.ForegroundColor = ConsoleColor.Magenta;
          Console.Error.Write("[Cri]");
          break;
        }

        case 4: { // Error
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Error.Write("[Err}");
          break;
        }

        case 3: { // Warning
          Console.ForegroundColor = ConsoleColor.DarkYellow;
          Console.Write("[Wrn]"); // Warnings go through StdOut, because they usually don't abort higher level actions
          break;
        }

        case 2: { // Information
          Console.Write("[Inf]");
          break;
        }

        case 1: { // Debug
          Console.ForegroundColor = ConsoleColor.DarkCyan;
          Console.Write("[Dbg]"); // 0 Trace
          break;
        }

        default: {
          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.Write("[Trc]");
          break;
        }

      }

      // todo zentralisieren und stringbuilder

      Console.Write(" ");
      Console.Write(sourceContext);
      Console.Write(" #");
      Console.Write(eventId.ToString());
      Console.Write("# ");

      Console.Write(":");

      // string message = messageTemplate.TryResolvePlaceholders(messageArgs);

      Console.WriteLine("message");

      Console.ForegroundColor = rescuedColor;

    }
  }
}
