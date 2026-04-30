using System;
using UnityEngine;

namespace Speedup.Services.Apple
{
    public class AppleCloudSave
    {
        public void SaveToCloud(string filename, string jsonData, Action<bool> onComplete)
        {
            // Note: Native iOS iCloud document/key-value sync is not built into UnityEngine.SocialPlatforms.
            // A third-party native plugin (like Prime31 or EasyMobile) is typically required for true iCloud support.
            // We use PlayerPrefs here as a local fallback so the interface is satisfied without crashing.
            Debug.LogWarning("[AppleCloudSave] Native iCloud save requires an external plugin. Using PlayerPrefs as a local fallback.");
            
            PlayerPrefs.SetString($"AppleCloudMock_{filename}", jsonData);
            PlayerPrefs.Save();
            
            onComplete?.Invoke(true);
        }

        public void LoadFromCloud(string filename, Action<bool, string> onComplete)
        {
            Debug.LogWarning("[AppleCloudSave] Native iCloud load requires an external plugin. Using PlayerPrefs as a local fallback.");
            
            string key = $"AppleCloudMock_{filename}";
            if (PlayerPrefs.HasKey(key))
            {
                string data = PlayerPrefs.GetString(key);
                onComplete?.Invoke(true, data);
            }
            else
            {
                onComplete?.Invoke(false, null);
            }
        }
    }
}
