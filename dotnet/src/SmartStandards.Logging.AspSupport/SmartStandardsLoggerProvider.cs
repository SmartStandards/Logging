﻿using Logging.SmartStandards.Centralized;
using Logging.SmartStandards.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Logging.SmartStandards.AspSupport {

  public static class AspLoggingSetupExtensions {

    /// <summary>
    /// Configures the ASP.NET Core logging system to initialize and use proxy-loggers,
    /// which will redirect any log events into the SmartStandards DevLogger AND
    /// does also setup/wireup the LogEntryCreator with the given configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="applicationName"></param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AddSmartStandardsLogging(this IServiceCollection services, IConfiguration configuration, string applicationName = null) {

      if (string.IsNullOrWhiteSpace(applicationName)) {
        applicationName = Assembly.GetCallingAssembly().GetName().Name;
      }

      LoggingConfiguration loggingConfiguration = configuration.GetSmartStandardsLoggingConfiguration();

      services.AddSmartStandardsLogging(loggingConfiguration, applicationName);

    }

    /// <summary>
    /// Configures the ASP.NET Core logging system to initialize and use proxy-loggers,
    /// which will redirect any log events into the SmartStandards DevLogger AND
    /// does also setup/wireup the LogEntryCreator with the given configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="loggingConfiguration"></param>
    /// <param name="applicationName"></param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AddSmartStandardsLogging(this IServiceCollection services, LoggingConfiguration loggingConfiguration, string applicationName = null) {

      if (string.IsNullOrWhiteSpace(applicationName)) {
        LogEntryCreator.ApplicationName = Assembly.GetCallingAssembly().GetName().Name;
      }
      else {
        LogEntryCreator.ApplicationName = applicationName;
      }

      services.AddLogging((ILoggingBuilder loggingBuilder) => {
        loggingBuilder.UseSmartStandardsLogging(loggingConfiguration);
      });

    }

    /// <summary>
    /// Gets a SmartStandards LoggingConfiguration from the given IConfiguration instance.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static LoggingConfiguration GetSmartStandardsLoggingConfiguration(this IConfiguration configuration) {

      LoggingConfiguration loggingConfiguration = (
        configuration.GetSection("Logging").GetSection("SmartStandards").Get<LoggingConfiguration>()
      );
      
      return loggingConfiguration;

    }

    /// <summary>
    /// Configures the ASP.NET Core logging system to initialize and use proxy-loggers,
    /// which will redirect any log events into the SmartStandards DevLogger AND
    /// does also setup/wireup the LogEntryCreator with the given configuration.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configurationToSetup" >
    /// 
    /// </param >
    public static void UseSmartStandardsLogging(this ILoggingBuilder builder, LoggingConfiguration configurationToSetup) {

      builder.ClearProviders();
      builder.AddProvider(new SmartStandardsLoggerProvider());

      if(configurationToSetup == null) {
        configurationToSetup = new LoggingConfiguration();
      }

      LogEntryCreator.Setup(
        configurationToSetup
      );

    }

    /// <summary>
    /// Configures the ASP.NET Core logging system to initialize and use proxy-loggers,
    /// which will redirect any log events into the SmartStandards DevLogger.
    /// NOTE: this overload does not setup the LogEntryCreator - if this is required,
    /// please use the one, where you can provide a LoggingConfiguration.
    /// </summary>
    /// <param name="builder"></param>
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

      string message = formatter(state, exception);
      if(exception == null) {
        DevLogger.Log(smartStandardsLevel, _Category, 0, eventId.Id, message);
      }
      else {
        DevLogger.Log(smartStandardsLevel, _Category, 0, eventId.Id, exception.Wrap(message));      
      }
      
    }

  }

}
