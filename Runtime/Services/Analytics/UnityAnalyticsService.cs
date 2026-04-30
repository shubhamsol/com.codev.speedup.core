using System.Collections.Generic;
using UnityEngine;
using Speedup.Core;

namespace Speedup.Services.Analytics
{
    /// <summary>
    /// Wrapper for Unity Analytics. Stubbed out until the package is installed.
    /// To use real analytics, install the Unity Analytics package and wire it up here.
    /// </summary>
    public class UnityAnalyticsService : IAnalyticsService
    {
        public void Initialize()
        {
            Debug.Log("[UnityAnalyticsService] Initialized (Stub). Waiting for Analytics package.");
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            Debug.Log($"[UnityAnalyticsService] LogEvent: {eventName}");
            // e.g. AnalyticsService.Instance.RecordEvent(eventName, parameters);
        }

        public void SetUserProperty(string propertyName, string value)
        {
            Debug.Log($"[UnityAnalyticsService] SetUserProperty: {propertyName} = {value}");
        }
    }
}
