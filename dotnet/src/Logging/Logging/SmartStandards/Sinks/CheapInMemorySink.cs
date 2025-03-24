using System.Collections.Generic;

namespace Logging.SmartStandards {

  public class CheapInMemorySink {

    public List<string> CollectedAudiences { get; set; } = new List<string>();

    public List<int> CollectedLevels { get; set; } = new List<int>();

    public List<string> CollectedSourceContexts { get; set; } = new List<string>();

    public List<long> CollectedSourceLineIds { get; set; } = new List<long>();

    public List<int> CollectedEventIds { get; set; } = new List<int>();

    public List<string> CollectedMessageTemplates { get; set; } = new List<string>();

    public List<object[]> CollectedMessageArgs { get; set; } = new List<object[]>();

    public void Clear() {
      this.CollectedAudiences.Clear();
      this.CollectedLevels.Clear();
      this.CollectedSourceContexts.Clear();
      this.CollectedSourceLineIds.Clear();
      this.CollectedEventIds.Clear();
      this.CollectedMessageTemplates.Clear();
      this.CollectedMessageArgs.Clear();
    }

    public void WriteLogEntry(
      string audience, int level, string sourceContext, long sourceLineId,
      int eventId, string messageTemplate, object[] args
    ) {
      this.CollectedAudiences.Add(audience);
      this.CollectedLevels.Add(level);
      this.CollectedSourceContexts.Add(sourceContext);
      this.CollectedSourceLineIds.Add(sourceLineId);
      this.CollectedEventIds.Add(eventId);
      this.CollectedMessageTemplates.Add(messageTemplate);
      this.CollectedMessageArgs.Add(args);
    }
  }
}
