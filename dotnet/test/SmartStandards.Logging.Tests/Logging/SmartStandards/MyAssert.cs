using Logging.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Logging.SmartStandards {

  internal class MyAssert {

    public static void CustomBusSinkContains(
      int index, string audience, int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate,
      Exception ex = null
    ) {
      Assert.AreEqual(audience, AssemblyInitializer.CustomBusSink.CollectedAudiences[index]);
      Assert.AreEqual(level, AssemblyInitializer.CustomBusSink.CollectedLevels[index]);
      Assert.AreEqual(sourceContext, AssemblyInitializer.CustomBusSink.CollectedSourceContexts[index]);
      Assert.AreEqual(sourceLineId, AssemblyInitializer.CustomBusSink.CollectedSourceLineIds[index]);
      Assert.AreEqual(kindId, AssemblyInitializer.CustomBusSink.CollectedEventIds[index]);
      Assert.AreEqual(messageTemplate, AssemblyInitializer.CustomBusSink.CollectedMessageTemplates[index]);

      if (ex != null) {
        Assert.AreEqual(ex, AssemblyInitializer.CustomBusSink.CollectedExceptions[index]);
      }

    }

    public static void TraceBusSinkContains(
      int index, string audience, int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate,
      Exception ex = null
    ) {
      Assert.AreEqual(audience, AssemblyInitializer.TraceBusSink.CollectedAudiences[index]);
      Assert.AreEqual(level, AssemblyInitializer.TraceBusSink.CollectedLevels[index]);
      Assert.AreEqual(sourceContext, AssemblyInitializer.TraceBusSink.CollectedSourceContexts[index]);
      Assert.AreEqual(sourceLineId, AssemblyInitializer.TraceBusSink.CollectedSourceLineIds[index]);
      Assert.AreEqual(kindId, AssemblyInitializer.TraceBusSink.CollectedEventIds[index]);
      Assert.AreEqual(messageTemplate, AssemblyInitializer.TraceBusSink.CollectedMessageTemplates[index]);

      if (ex != null) {
        Assert.AreEqual(ex, AssemblyInitializer.CustomBusSink.CollectedExceptions[index]);
      }
    }

    public static void BothSinksContain(
      int index, string audience, int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate,
      Exception ex = null
    ) {
      TraceBusSinkContains(index, audience, level, sourceContext, sourceLineId, kindId, messageTemplate, ex);
      CustomBusSinkContains(index, audience, level, sourceContext, sourceLineId, kindId, messageTemplate, ex);
    }

  }
}
