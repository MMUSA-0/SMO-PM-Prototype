using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Abstractions
{
    public interface ILoggingService
    {
        void LogUserAction(string action, string details, string userId);
        void LogError(string message, Exception exception, string userId = null);
        void LogSecurityEvent(string eventType, string details, string userId = null);
        void LogPerformance(string operation, long durationMs, string userId = null);
        void LogApplicationEvent(string eventType, string message, string userId = null);
    }
}
