using System;
using System.Collections.Generic;

namespace Logging.SmartStandards.Sinks {

  public class CheapInMemorySink {

    public List<string> CollectedAudiences { get; set; } = new List<string>();

    public List<int> CollectedLevels { get; set; } = new List<int>();

    public List<string> CollectedSourceContexts { get; set; } = new List<string>();

    public List<long> CollectedSourceLineIds { get; set; } = new List<long>();

    public List<int> CollectedEventIds { get; set; } = new List<int>();

    public List<string> CollectedMessageTemplates { get; set; } = new List<string>();

    public List<object[]> CollectedMessageArgs { get; set; } = new List<object[]>();

    public List<Exception> CollectedExceptions { get; set; } = new List<Exception>();

    public void Clear() {
      this.CollectedAudiences.Clear();
      this.CollectedLevels.Clear();
      this.CollectedSourceContexts.Clear();
      this.CollectedSourceLineIds.Clear();
      this.CollectedEventIds.Clear();
      this.CollectedMessageTemplates.Clear();
      this.CollectedMessageArgs.Clear();
    }

    public void WriteMessage(
      string audience, int level, string sourceContext, long sourceLineId,
      int kindId, string messageTemplate, object[] args
    ) {
      this.CollectedAudiences.Add(audience);
      this.CollectedLevels.Add(level);
      this.CollectedSourceContexts.Add(sourceContext);
      this.CollectedSourceLineIds.Add(sourceLineId);
      this.CollectedEventIds.Add(kindId);
      this.CollectedMessageTemplates.Add(messageTemplate);
      this.CollectedMessageArgs.Add(args);
      this.CollectedExceptions.Add(null);
    }

    public void WriteException(
      string audience, int level, string sourceContext, long sourceLineId,
      int kindId, Exception ex
    ) {
      this.CollectedAudiences.Add(audience);
      this.CollectedLevels.Add(level);
      this.CollectedSourceContexts.Add(sourceContext);
      this.CollectedSourceLineIds.Add(sourceLineId);
      this.CollectedEventIds.Add(kindId);
      this.CollectedMessageTemplates.Add(null);
      this.CollectedMessageArgs.Add(null);
      this.CollectedExceptions.Add(ex);
    }
  }
}
