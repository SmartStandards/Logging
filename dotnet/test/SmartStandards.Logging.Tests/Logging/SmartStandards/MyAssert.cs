using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Logging.SmartStandards {

  internal class MyAssert {

    public static void CustomBusSinkContains(
      int index, string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate,
      Exception ex = null
    ) {
      Assert.AreEqual(audience, AssemblyInitializer.CustomBusSink.CollectedAudiences[index]);
      Assert.AreEqual(level, AssemblyInitializer.CustomBusSink.CollectedLevels[index]);
      Assert.AreEqual(sourceContext, AssemblyInitializer.CustomBusSink.CollectedSourceContexts[index]);
      Assert.AreEqual(sourceLineId, AssemblyInitializer.CustomBusSink.CollectedSourceLineIds[index]);
      Assert.AreEqual(eventKindId, AssemblyInitializer.CustomBusSink.CollectedEventKindIds[index]);
      Assert.AreEqual(messageTemplate, AssemblyInitializer.CustomBusSink.CollectedMessageTemplates[index]);

      if (ex != null) {
        Assert.AreEqual(ex, AssemblyInitializer.CustomBusSink.CollectedExceptions[index]);
      }

    }

    public static void RawTraceBusSinkContains(
      int index, string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate,
      Exception ex = null
    ) {
      Assert.AreEqual(audience, AssemblyInitializer.RawTraceBusSink.CollectedAudiences[index]);
      Assert.AreEqual(level, AssemblyInitializer.RawTraceBusSink.CollectedLevels[index]);
      Assert.AreEqual(sourceContext, AssemblyInitializer.RawTraceBusSink.CollectedSourceContexts[index]);
      Assert.AreEqual(sourceLineId, AssemblyInitializer.RawTraceBusSink.CollectedSourceLineIds[index]);
      Assert.AreEqual(eventKindId, AssemblyInitializer.RawTraceBusSink.CollectedEventKindIds[index]);
      Assert.AreEqual(messageTemplate, AssemblyInitializer.RawTraceBusSink.CollectedMessageTemplates[index]);

      if (ex != null) {
        Assert.AreEqual(ex, AssemblyInitializer.RawTraceBusSink.CollectedExceptions[index]);
      }
    }

    public static void BothSinksContain(
      int index, string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate,
      Exception ex = null
    ) {
      RawTraceBusSinkContains(index, audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, ex);
      CustomBusSinkContains(index, audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, ex);
    }

    public static void ReadyToReadraceBusSinkContains(int index, string text) {
      Assert.AreEqual(text, AssemblyInitializer.ReadyToReadTraceBusSink.CollectedMessageTemplates[index]);
    }

  }
}
