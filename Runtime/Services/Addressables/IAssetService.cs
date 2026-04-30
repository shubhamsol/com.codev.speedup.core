using System;
using Speedup.Core;

namespace Speedup.Services.Addressables
{
    public interface IAssetService : IGameService
    {
        void LoadAssetAsync<T>(string address, Action<T> onComplete) where T : UnityEngine.Object;
        void InstantiateAsync(string address, Action<UnityEngine.GameObject> onComplete);
        void ReleaseAsset(object asset);
    }
}
