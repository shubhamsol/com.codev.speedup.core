using System;
using Speedup.Core;

namespace Speedup.Services.Save
{
    public interface ISaveService : IGameService
    {
        void Save(Action<bool> onComplete = null, bool forceFullSnapshot = false);
        void Save(string slotId, Action<bool> onComplete = null, bool forceFullSnapshot = false);
        void Load(Action<bool> onComplete = null);
        void Load(string slotId, Action<bool> onComplete = null);
    }
}
