using System.Collections.Generic;

namespace Logging.SmartStandards {

  public static class CheapInMemoryLogger {

    public static List<string> CollectedChannels { get; set; } = new List<string>();

    public static List<int> CollectedLevels { get; set; } = new List<int>();

    public static List<int> CollectedIds { get; set; } = new List<int>();

    public static List<string> CollectedMessageTemplates { get; set; } = new List<string>();

    public static List<string[]> CollectedMessageArgs { get; set; } = new List<string[]>();

    public static bool _IsActive;

    public static bool _IsInitialized;

    public static void Start() {
      Initialize();
      Clear();
      _IsActive = true;
    }

    public static void Stop() {

      if (!_IsInitialized) return;

      _IsActive = false;

      Clear();

    }

    public static void Clear() {

      if (!_IsInitialized) return;

      CollectedChannels.Clear();
      CollectedLevels.Clear();
      CollectedIds.Clear();
      CollectedMessageTemplates.Clear();
      CollectedMessageArgs.Clear();

    }

    private static void Initialize() {

      if (_IsInitialized) return;

      SmartStandardsTraceLogPipe.Initialize(CheapInMemoryLogger.OnLog);

      _IsInitialized = true;
    }

    private static void OnLog(string channelName, int level, int id, string messageTemplate, string[] messageArgs) {

      if (!_IsActive) return;

      CollectedChannels.Add(channelName);
      CollectedLevels.Add(level);
      CollectedIds.Add(id);
      CollectedMessageTemplates.Add(messageTemplate);
      CollectedMessageArgs.Add(messageArgs);
    }

  }

}
