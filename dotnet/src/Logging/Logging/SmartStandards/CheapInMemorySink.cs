using System.Collections.Generic;

namespace Logging.SmartStandards {

  public static class CheapInMemorySink {

    public static bool UseTrace;

    private static bool _TraceIsInitialized;

    private static int _CurrentlyUsingDirectOrTrace = 0;

    private static bool _IsActive;

    public static List<string> CollectedPipes { get; set; } = new List<string>();

    public static List<string> CollectedChannels { get; set; } = new List<string>();

    public static List<int> CollectedLevels { get; set; } = new List<int>();

    public static List<int> CollectedIds { get; set; } = new List<int>();

    public static List<string> CollectedMessageTemplates { get; set; } = new List<string>();

    public static List<object[]> CollectedMessageArgs { get; set; } = new List<object[]>();

    public static void Start() {
      Initialize();
      Clear();
      _IsActive = true;
    }

    public static void Stop() {

      _IsActive = false;

      Clear();
    }

    public static void Clear() {

      CollectedPipes.Clear();
      CollectedChannels.Clear();
      CollectedLevels.Clear();
      CollectedIds.Clear();
      CollectedMessageTemplates.Clear();
      CollectedMessageArgs.Clear();

    }

    private static void Initialize() {

      if (UseTrace) {

        if (_CurrentlyUsingDirectOrTrace != 2) {

          if (!_TraceIsInitialized) {
            SmartStandardsTraceLogPipe.Initialize((channelName, level, id, messageTemplate, args) => OnLog("trace", channelName, level, id, messageTemplate, args));
            _TraceIsInitialized = true;
          }

          DevLogger.ConfigureRedirection(null, null, false, false);
          InfrastructureLogger.ConfigureRedirection(null, null, false, false);
          ProtocolLogger.ConfigureRedirection(null, null, false, false);

          _CurrentlyUsingDirectOrTrace = 2;
        }

      } else {

        if (_CurrentlyUsingDirectOrTrace != 1) {

          DevLogger.ConfigureRedirection((channelName, level, id, messageTemplate, args) => OnLog("direct", channelName, level, id, messageTemplate, args), null, false, false);
          InfrastructureLogger.ConfigureRedirection((channelName, level, id, messageTemplate, args) => OnLog("direct", channelName, level, id, messageTemplate, args), null, false, false);
          ProtocolLogger.ConfigureRedirection((channelName, level, id, messageTemplate, args) => OnLog("direct", channelName, level, id, messageTemplate, args), null, false, false);

        }

        _CurrentlyUsingDirectOrTrace = 1;
      }
    }

    private static void OnLog(string pipeKind, string channelName, int level, int id, string messageTemplate, object[] messageArgs) {

      if (!_IsActive) return;

      CollectedPipes.Add(pipeKind);
      CollectedChannels.Add(channelName);
      CollectedLevels.Add(level);
      CollectedIds.Add(id);
      CollectedMessageTemplates.Add(messageTemplate);
      CollectedMessageArgs.Add(messageArgs);
    }

  }

}
