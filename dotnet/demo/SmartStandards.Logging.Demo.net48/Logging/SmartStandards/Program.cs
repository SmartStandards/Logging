using System;
using System.Diagnostics;
using System.Threading;

namespace Logging.SmartStandards {

  internal class Program {

    static void Main(string[] args) {

      Trace.WriteLine("■Trace.WriteLine()■");
      Trace.TraceInformation("■Trace.TraceInformation()■");

      // Initialization

      Console.CancelKeyPress += OnCancelKeyPress;

      AppDomain.CurrentDomain.ProcessExit += OnProcessExit; // not executing?

      Routing.UseConsoleSink();

      // Start

      Console.WriteLine("Syntax:");
      Console.WriteLine("[LevelAsAlpha3] SourceContext #UseCaseId# SourceLineId [AudienceToken]: ResolvedMessageTemplate");

      // Minimum Example

      Console.WriteLine();
      Console.WriteLine("-- Minimum Examples --");
      Console.WriteLine("(SourceContext defaulted to current assemlby name.)");
      Console.WriteLine();

      DevLogger.LogCritical("■DevLogger.LogCritical()■");
      DevLogger.LogError("■DevLogger.LogError()■");
      DevLogger.LogWarning("■DevLogger.LogWarning()■");
      DevLogger.LogInformation("■DevLogger.LogInformation()■");
      DevLogger.LogDebug("■DevLogger.LogDebug()■");
      DevLogger.LogTrace("■DevLogger.LogTrace()■");

      // Maximum Example

      Console.WriteLine();
      Console.WriteLine("-- Maximum Example --");
      Console.WriteLine();

      InsLogger.LogInformation(
        "MySourceContext", 2073052666173024241, 42, "Text is {text}, number is {number}.", "Foo", 123
      );

      Console.WriteLine();
      Console.WriteLine("-- Message Coming From LogUseCaseEnum Example --");
      Console.WriteLine();

      BizLogger.LogInformation("MySourceContext", 2073054001366991604, MyLogUseCase.ZuVielFooImBar, "Batz");

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
      //Console.WriteLine($"There were {Routing.InternalTraceBusFeed.ListenersActive.Count} TraceListeners active.");
      //Console.WriteLine($"Name is {Routing.InternalTraceBusFeed.ListenersActive[0].Name}");
      //Console.WriteLine($"Type is {Routing.InternalTraceBusFeed.ListenersActive[0].GetType().Name}");
      Console.WriteLine("-- Done. --");
      Console.WriteLine();

      for (int i = 0; i < 500; i++) {
        DevLogger.LogTrace($"■{DateTime.Now}■");
        Thread.Sleep(500);
      }

    }

    static private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e) {
      Trace.WriteLine("■OnCancelKeyPress()■");
    }

    static private void OnProcessExit(object sender, EventArgs e) {
      Trace.WriteLine("■OnProcessExit()■");
    }

  }
}
