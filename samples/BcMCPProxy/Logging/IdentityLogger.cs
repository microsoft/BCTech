namespace BcMCPProxy.Logging
{
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Abstractions;
    using System;

    public class IdentityLogger : IIdentityLogger
    {
        private readonly ILogger logger;

        public IdentityLogger(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<IdentityLogger>();
        }

        public bool IsEnabled(EventLogLevel eventLogLevel)
        {
            return logger.IsEnabled(ToLogLevel(eventLogLevel));
        }

        public void Log(LogLevel level, string message)
        {
            logger.Log(level, message);
        }

        public void Log(LogEntry entry)
        {
            logger.Log(ToLogLevel(entry.EventLogLevel), entry.Message);
        }

        public LogLevel ToLogLevel(EventLogLevel eventLogLevel)
        {
            return eventLogLevel switch
            {
                EventLogLevel.LogAlways => LogLevel.Trace,
                EventLogLevel.Critical => LogLevel.Critical,
                EventLogLevel.Error => LogLevel.Error,
                EventLogLevel.Warning => LogLevel.Warning,
                EventLogLevel.Informational => LogLevel.Information,
                EventLogLevel.Verbose => LogLevel.Debug,
                _ => throw new ArgumentOutOfRangeException(nameof(eventLogLevel), eventLogLevel, null)
            };
        }
    }
}
