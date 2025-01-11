using System;

namespace Logging.SmartStandards {

  public class CustomBusFeed {

    public delegate void EmitMessageDelegate(string sourceContext, string audience, int level, int id, string messageTemplate, object[] args);

    public delegate void EmitExceptionDelegate(string sourceContext, string audience, int level, Exception ex);

    public static EmitMessageDelegate OnEmitMessage { get; set; }

    public static EmitExceptionDelegate OnEmitException { get; set; }

  }

}
