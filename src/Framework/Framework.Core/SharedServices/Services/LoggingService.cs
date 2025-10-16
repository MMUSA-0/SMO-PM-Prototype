using Framework.Core.SharedServices.Abstractions;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Services
{
    internal class LoggingService : ILoggingService
    {
        private static readonly ILogger UserActionLogger = LogManager.GetLogger("UserActions");
        private static readonly ILogger ErrorLogger = LogManager.GetLogger("Error");
        private static readonly ILogger SecurityLogger = LogManager.GetLogger("Security");
        private static readonly ILogger PerformanceLogger = LogManager.GetLogger("Performance");
        private static readonly ILogger AppLogger = LogManager.GetLogger("App");

        public void LogUserAction(string action, string details, string userId)
        {
            var logEvent = new LogEventInfo(LogLevel.Info, "UserActions", $"{action} - {details}");
            logEvent.Properties["UserId"] = userId;
            UserActionLogger.Log(logEvent);
        }

        public void LogError(string message, Exception exception, string userId = null)
        {
            var logEvent = new LogEventInfo(LogLevel.Error, "Error", message);
            logEvent.Exception = exception;
            if (!string.IsNullOrEmpty(userId))
            {
                logEvent.Properties["UserId"] = userId;
            }
            ErrorLogger.Log(logEvent);
        }

        public void LogSecurityEvent(string eventType, string details, string userId = null)
        {
            var logEvent = new LogEventInfo(LogLevel.Info, "Security", $"{eventType} - {details}");
            if (!string.IsNullOrEmpty(userId))
            {
                logEvent.Properties["UserId"] = userId;
            }
            SecurityLogger.Log(logEvent);
        }

        public void LogPerformance(string operation, long durationMs, string userId = null)
        {
            var logEvent = new LogEventInfo(LogLevel.Info, "Performance", $"{operation} completed in {durationMs}ms");
            if (!string.IsNullOrEmpty(userId))
            {
                logEvent.Properties["UserId"] = userId;
            }
            PerformanceLogger.Log(logEvent);
        }

        public void LogApplicationEvent(string eventType, string message, string userId = null)
        {
            var logEvent = new LogEventInfo(LogLevel.Info, "App", $"{eventType} - {message}");
            if (!string.IsNullOrEmpty(userId))
            {
                logEvent.Properties["UserId"] = userId;
            }
            AppLogger.Log(logEvent);
        }
    }
}
