using System;

namespace Speedup.Services.Save
{
    public interface ISaveProvider
    {
        string ProviderId { get; }
        bool IsAvailable { get; }
        void Save(SaveContext context, string payload, Action<bool> onComplete);
        void Load(SaveContext context, Action<bool, string> onComplete);
    }
}
