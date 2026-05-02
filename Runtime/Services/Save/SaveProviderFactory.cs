using System.Collections.Generic;
using Speedup.Services.Save.Providers;

namespace Speedup.Services.Save
{
    public static class SaveProviderFactory
    {
        public static List<ISaveProvider> CreateDefaultFallbackChain()
        {
            var providers = new List<ISaveProvider>();

#if UNITY_IOS
            providers.Add(new AppleCloudSaveProvider());
            providers.Add(new LocalSaveProvider());
#elif UNITY_ANDROID
            providers.Add(new GooglePlayCloudSaveProvider());
            providers.Add(new LocalSaveProvider());
#else
            providers.Add(new LocalSaveProvider());
#endif
            return providers;
        }
    }
}
