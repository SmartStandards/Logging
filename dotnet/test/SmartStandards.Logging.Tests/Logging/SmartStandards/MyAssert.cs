using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

    public static void TraceBusSinkContains(
      int index, string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate,
      Exception ex = null
    ) {
      Assert.AreEqual(audience, AssemblyInitializer.TraceBusSink.CollectedAudiences[index]);
      Assert.AreEqual(level, AssemblyInitializer.TraceBusSink.CollectedLevels[index]);
      Assert.AreEqual(sourceContext, AssemblyInitializer.TraceBusSink.CollectedSourceContexts[index]);
      Assert.AreEqual(sourceLineId, AssemblyInitializer.TraceBusSink.CollectedSourceLineIds[index]);
      Assert.AreEqual(eventKindId, AssemblyInitializer.TraceBusSink.CollectedEventKindIds[index]);
      Assert.AreEqual(messageTemplate, AssemblyInitializer.TraceBusSink.CollectedMessageTemplates[index]);

      if (ex != null) {
        Assert.AreEqual(ex, AssemblyInitializer.TraceBusSink.CollectedExceptions[index]);
      }
    }

    public static void BothSinksContain(
      int index, string audience, int level, string sourceContext, long sourceLineId, int eventKindId, string messageTemplate,
      Exception ex = null
    ) {
      TraceBusSinkContains(index, audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, ex);
      CustomBusSinkContains(index, audience, level, sourceContext, sourceLineId, eventKindId, messageTemplate, ex);
    }

  }
}
