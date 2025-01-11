using System.Collections.Generic;

namespace Logging.SmartStandards {

  public static class CheapInMemorySink {

    public static List<string> CollectedSourceContexts { get; set; } = new List<string>();

    public static List<string> CollectedAudiences { get; set; } = new List<string>();

    public static List<int> CollectedLevels { get; set; } = new List<int>();

    public static List<int> CollectedIds { get; set; } = new List<int>();

    public static List<string> CollectedMessageTemplates { get; set; } = new List<string>();

    public static List<object[]> CollectedMessageArgs { get; set; } = new List<object[]>();

    public static void Clear() {
      CollectedSourceContexts.Clear();
      CollectedAudiences.Clear();
      CollectedLevels.Clear();
      CollectedIds.Clear();
      CollectedMessageTemplates.Clear();
      CollectedMessageArgs.Clear();
    }

    public static void Log(string sourceContext, string audience, int level, int id, string messageTemplate, object[] messageArgs) {
      CollectedSourceContexts.Add(sourceContext);
      CollectedAudiences.Add(audience);
      CollectedLevels.Add(level);
      CollectedIds.Add(id);
      CollectedMessageTemplates.Add(messageTemplate);
      CollectedMessageArgs.Add(messageArgs);
    }
  }
}
