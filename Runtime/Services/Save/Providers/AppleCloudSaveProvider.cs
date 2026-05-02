using System;
using Speedup.Services.Apple;

namespace Speedup.Services.Save.Providers
{
    public class AppleCloudSaveProvider : ISaveProvider
    {
        private readonly AppleCloudSave _cloudSave = new AppleCloudSave();

        public string ProviderId => "apple-cloud";
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
