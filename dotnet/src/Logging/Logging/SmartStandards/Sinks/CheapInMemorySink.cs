using System.Collections.Generic;

namespace Logging.SmartStandards {

  public static class CheapInMemorySink {

    public static List<string> CollectedAudiences { get; set; } = new List<string>();

    public static List<int> CollectedLevels { get; set; } = new List<int>();

    public static List<string> CollectedSourceContexts { get; set; } = new List<string>();

    public static List<long> CollectedSourceLineIds { get; set; } = new List<long>();

    public static List<int> CollectedEventIds { get; set; } = new List<int>();

    public static List<string> CollectedMessageTemplates { get; set; } = new List<string>();

    public static List<object[]> CollectedMessageArgs { get; set; } = new List<object[]>();

    public static void Clear() {
      CollectedSourceContexts.Clear();
      CollectedAudiences.Clear();
      CollectedLevels.Clear();
      CollectedEventIds.Clear();
      CollectedMessageTemplates.Clear();
      CollectedMessageArgs.Clear();
    }

    public static void WriteLogEntry(
      string audience, int level, string sourceContext, long sourceLineId, 
      int eventId, string messageTemplate, object[] args
    ) {
      CollectedAudiences.Add(audience);
      CollectedLevels.Add(level);
      CollectedSourceContexts.Add(sourceContext);
      CollectedSourceLineIds.Add(sourceLineId);
      CollectedEventIds.Add(eventId);
      CollectedMessageTemplates.Add(messageTemplate);
      CollectedMessageArgs.Add(args);
    }
  }
}
