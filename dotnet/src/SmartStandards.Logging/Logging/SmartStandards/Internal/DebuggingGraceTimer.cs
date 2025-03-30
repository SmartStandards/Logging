using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.SmartStandards.Internal {

  internal static class DebuggingGraceTimer {

    internal static bool IsCancelled { get; set; } = true;

    private static void DisableEmittingToTraceBusAfter10Seconds() {

      Thread.Sleep(10000);

      if (!IsCancelled && !Debugger.IsAttached) {
        Routing.EnableEmittingToTraceBus(false);
      }

    }

    internal static void Start() {

      Task.Run(DisableEmittingToTraceBusAfter10Seconds);

    }

  }

}
