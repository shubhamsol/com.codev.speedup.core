using System.Collections.Generic;
using Speedup.Core;

namespace Speedup.Services.Analytics
{
    public interface IAnalyticsService : IGameService
    {
        void LogEvent(string eventName, Dictionary<string, object> parameters = null);
        void SetUserProperty(string propertyName, string value);
    }
}
