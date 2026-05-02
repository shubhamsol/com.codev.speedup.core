using System;
using UnityEngine;

namespace Speedup.Services.Save.Providers
{
    public class LocalSaveProvider : ISaveProvider
    {
        private readonly string _keyPrefix;

        public string ProviderId => "local";
        public bool IsAvailable => true;

        public LocalSaveProvider(string keyPrefix = "Speedup.Save")
        {
            _keyPrefix = string.IsNullOrWhiteSpace(keyPrefix) ? "Speedup.Save" : keyPrefix;
        }

        public void Save(SaveContext context, string payload, Action<bool> onComplete)
        {
            string key = BuildStorageKey(context);
            PlayerPrefs.SetString(key, payload);
            PlayerPrefs.Save();
            onComplete?.Invoke(true);
        }

        public void Load(SaveContext context, Action<bool, string> onComplete)
        {
            string key = BuildStorageKey(context);
            if (!PlayerPrefs.HasKey(key))
            {
                onComplete?.Invoke(false, null);
                return;
            }

            onComplete?.Invoke(true, PlayerPrefs.GetString(key));
        }

        private string BuildStorageKey(SaveContext context)
        {
            return $"{_keyPrefix}.{context.ProfileId}.{context.SlotId}";
        }
    }
}
