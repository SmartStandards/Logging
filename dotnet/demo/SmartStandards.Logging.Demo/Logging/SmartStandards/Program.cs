using System;
using System.Diagnostics;

namespace Logging.SmartStandards {

  internal class Program {

    static void Main(string[] args) {

      Trace.WriteLine("How can this WriteLine() reach VS Output without anything in Trace.Listeners?");
      Trace.TraceInformation("How can this TraceInformation() reach VS Output without anything in Trace.Listeners?");

      // Initialization

      Routing.UseConsoleSink();

      // Start

      Console.WriteLine("Syntax:");
      Console.WriteLine("[LevelAsAlpha3] SourceContext #KindId# SourceLineId [AudienceToken]: ResolvedMessageTemplate");

      // Minimum Example

      Console.WriteLine();
      Console.WriteLine("-- Minimum Examples --");
      Console.WriteLine("(SourceContext defaulted to current assemlby name.)");
      Console.WriteLine();

      DevLogger.LogCritical("Hello World.");
      DevLogger.LogError("Hello World.");
      DevLogger.LogWarning("Hello World.");
      DevLogger.LogInformation("Hello World.");
      DevLogger.LogDebug("Hello World.");
      DevLogger.LogTrace("Hello World.");

      // Maximum Example

      Console.WriteLine();
      Console.WriteLine("-- Maximum Example --");
      Console.WriteLine();

      InsLogger.LogInformation(
        "MySourceContext", 2073052666173024241, 42, "Text is {text}, number is {number}.", "Foo", 123
      );

      Console.WriteLine();
      Console.WriteLine("-- LogEventTemplate Coming From Enum Example --");
      Console.WriteLine();

      BizLogger.LogInformation("MySourceContext", 2073054001366991604, MyLogTemplate.ZuVielFooImBar, "Batz");

      // Exception Example

      Console.WriteLine();
      Console.WriteLine("-- Exception Example --");
      Console.WriteLine();

      Exception caughtException = null;

      int numberOfParticipants = 0;
      try {
        int boom = 100 / numberOfParticipants;
      } catch (Exception e) {
        caughtException = e;
      }

      Exception enrichedException = caughtException.Wrap(
        87, $"NumberOfParticipants was {numberOfParticipants}, which caused the inner Exception."
      );

      DevLogger.LogCritical("MySourceContext", 2073054166198233317, enrichedException);

      // Done.

      Console.WriteLine();
      Console.WriteLine($"There were {Routing.InternalTraceBusFeed.ListenersActive.Count} TraceListeners active.");
      Console.WriteLine($"Name is {Routing.InternalTraceBusFeed.ListenersActive[0].Name}");
      Console.WriteLine($"Type is {Routing.InternalTraceBusFeed.ListenersActive[0].GetType().Name}");
      Console.WriteLine("-- Done. --");
      Console.WriteLine();

    }
  }
}
