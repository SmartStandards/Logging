Error Processing 'https://raw.githubusercontent.com/SmartStandards/Logging/refs/heads/master/dotnet/src/SmartStandards.Logging/Logging/SmartStandards/Textualization/LogParaphRenderer.Minimum.cs': The remote server returned an error: (404) Not Found.

namespace Logging.SmartStandards {
 
  internal partial class DevLogger {

    public const string AudienceToken = "Dev";

    public static void Log(
      int level, string sourceContext, long sourceLineId, int kindId, string messageTemplate, params object[] args
    ) {

      if (string.IsNullOrWhiteSpace(sourceContext)) sourceContext = "UnknownSourceContext";

      if (messageTemplate == null) messageTemplate = "";

      if (args == null) args = new object[0];

      Logging.SmartStandards.Transport.TraceBusFeed.Instance.EmitMessage(AudienceToken, level, sourceContext, sourceLineId, kindId, messageTemplate, args);

    }

    public static void Log(int level, string sourceContext, long sourceLineId, Enum kindEnumElement, params object[] args) {
      TemplateHousekeeping.LogMessageTemplateRepository.GetMessageTemplateByKind(kindEnumElement, out int kindId, out string messageTemplate);
      Log(level, sourceContext, sourceLineId, kindId, messageTemplate, args);
    }

    public static void Log(int level, string sourceContext, long sourceLineId, Exception ex) {
      int kindId = Logging.SmartStandards.Internal.ExceptionAnalyzer.InferEventIdByException(ex);
      Logging.SmartStandards.Transport.TraceBusFeed.Instance.EmitException(AudienceToken, level, sourceContext, sourceLineId, kindId, ex); 
    }

  }
}

/*********************** SAMPLES **********************************
     
  [TypeConverter(typeof(LogMessageEnumConverter))]
  internal enum LogMessages {

    /// <summary>       "Weve got a Foo!" </summary>
    [LogMessageTemplate("Weve got a Foo!")]
    Foo = 110011,

    /// <summary>       "Weve got a Bar!" </summary>
    [LogMessageTemplate("Weve got a Bar!")]
    Bar = 220022

  }

  DevLogger.LogError(LogMessages.Bar);
  DevLogger.LogError(0, 2282, "A Freetext-Message");

  DevLogger.LogError(ex);
  DevLogger.LogError(ex.Wrap(22, "Another message"));
  DevLogger.LogError(ex.Wrap("Another message B"));
      
*/

