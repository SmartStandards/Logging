using System;
using Microsoft.Extensions.Logging;

namespace Logging.SmartStandards.AspSupport {

  public static class AspLoggingSetupExtensions {

    public static void UseSmartStandardsLogging(this ILoggingBuilder builder) {

      builder.ClearProviders();
      builder.AddProvider(new SmartStandardsLoggerProvider());

    }

  }

  public class SmartStandardsLoggerProvider : ILoggerProvider {

    public ILogger CreateLogger(string categoryName) {
      return new AspProxyLogger(categoryName);
    }

    public void Dispose() {
    }

  }

  public class AspProxyLogger : ILogger {

    private readonly string _Category;

    public AspProxyLogger(string category) {
      _Category = category;
    }


    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;


    public void Log<TState>(
      LogLevel logLevel, EventId eventId, TState state, Exception exception, 
      Func<TState, Exception, string> formatter
    ) {

      int smartStandardsLevel = 0;
      if(logLevel == LogLevel.Critical) {
        smartStandardsLevel = 5;
      }
      else if (logLevel == LogLevel.Error) {
        smartStandardsLevel = 4;
      }
      if (logLevel == LogLevel.Warning) {
        smartStandardsLevel = 3;
      }
      else if (logLevel == LogLevel.Information) {
        smartStandardsLevel = 2;
      }
      if (logLevel == LogLevel.Debug) {
        smartStandardsLevel = 1;
      }

      var message = formatter(state, exception);

      //TODO: ggf. extra Kanal für exception?
      //TODO: ist ein routing für andere andience nötig?
      DevLogger.Log(smartStandardsLevel, _Category, 0, eventId.Id, message);

    }

  }

}
