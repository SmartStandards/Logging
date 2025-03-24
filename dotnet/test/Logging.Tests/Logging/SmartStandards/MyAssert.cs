using Logging.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logging.SmartStandards {

  internal class MyAssert {

    public static void CustomBusSinkContains(
      int index, string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate
    ) {
      Assert.AreEqual(audience, AssemblyInitializer.CustomBusSink.CollectedAudiences[index]);
      Assert.AreEqual(level, AssemblyInitializer.CustomBusSink.CollectedLevels[index]);
      Assert.AreEqual(sourceContext, AssemblyInitializer.CustomBusSink.CollectedSourceContexts[index]);
      Assert.AreEqual(sourceLineId, AssemblyInitializer.CustomBusSink.CollectedSourceLineIds[index]);
      Assert.AreEqual(eventId, AssemblyInitializer.CustomBusSink.CollectedEventIds[index]);
      Assert.AreEqual(messageTemplate, AssemblyInitializer.CustomBusSink.CollectedMessageTemplates[index]);
    }

    public static void TraceBusSinkContains(
      int index, string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate
    ) {
      Assert.AreEqual(audience, AssemblyInitializer.TraceBusSink.CollectedAudiences[index]);
      Assert.AreEqual(level, AssemblyInitializer.TraceBusSink.CollectedLevels[index]);
      Assert.AreEqual(sourceContext, AssemblyInitializer.TraceBusSink.CollectedSourceContexts[index]);
      Assert.AreEqual(sourceLineId, AssemblyInitializer.TraceBusSink.CollectedSourceLineIds[index]);
      Assert.AreEqual(eventId, AssemblyInitializer.TraceBusSink.CollectedEventIds[index]);
      Assert.AreEqual(messageTemplate, AssemblyInitializer.TraceBusSink.CollectedMessageTemplates[index]);
    }

    public static void BothSinksContain(
      int index, string audience, int level, string sourceContext, long sourceLineId, int eventId, string messageTemplate
    ) {
      TraceBusSinkContains(index, audience, level, sourceContext, sourceLineId, eventId, messageTemplate);
      CustomBusSinkContains(index, audience, level, sourceContext, sourceLineId, eventId, messageTemplate);
    }

  }
}
