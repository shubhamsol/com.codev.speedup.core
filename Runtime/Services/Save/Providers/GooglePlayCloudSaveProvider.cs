using System;
using Speedup.Services.GPGS;

namespace Speedup.Services.Save.Providers
{
    public class GooglePlayCloudSaveProvider : ISaveProvider
    {
        private readonly GPGSCloudSave _cloudSave = new GPGSCloudSave();

        public string ProviderId => "google-cloud";
        public bool IsAvailable => true;

        public void Save(SaveContext context, string payload, Action<bool> onComplete)
        {
            _cloudSave.SaveToCloud(context.SlotId, payload, onComplete);
        }

        public void Load(SaveContext context, Action<bool, string> onComplete)
        {
            _cloudSave.LoadFromCloud(context.SlotId, onComplete);
        }
    }
}
