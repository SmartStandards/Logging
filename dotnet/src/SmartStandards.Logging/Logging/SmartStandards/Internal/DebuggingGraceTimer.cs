using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.SmartStandards.Internal {

  /// <summary>
  ///   Automatically turns off the emitting to trace bus after 10 seconds (because it's expensive).
  /// </summary>
  /// <remarks>
  ///   The (hard coded) default setting is to have emitting to trace bus enabled 
  ///   (lazily after the first log method call).
  ///   Within a time window of 10 seconds you can attach a debugger and you'll see all logged events in the immediate window.
  ///   If you don't, trace bus emitting will be turned off unless you touched any configuration property (or method) 
  ///   like ReEnableEmittingToTraceBus() or DevLoggerToTraceBus.
  ///   Calling any of these properties will disable the automatic turn off.
  /// </remarks>
  internal static class DebuggingGraceTimer {

    internal static bool IsCancelled { get; set; } = true;

    private static void DisableEmittingToTraceBusAfter10Seconds() {

      Thread.Sleep(10000);

      if (!IsCancelled && !Debugger.IsAttached) {
        Routing.ReEnableEmittingToTraceBus(false);
      }

    }

    internal static void Start() {

      Task.Run(DisableEmittingToTraceBusAfter10Seconds);

    }

  }

}
