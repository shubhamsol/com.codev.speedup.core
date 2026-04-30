using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Speedup.Core;

namespace Speedup.Services.Addressables
{
    /// <summary>
    /// Wrapper for Unity Addressables. 
    /// To enable, you must install the Addressables package from the Package Manager,
    /// and optionally wrap this in `#if USE_ADDRESSABLES` to toggle it.
    /// </summary>
    public class AddressableService : IAssetService
    {
        public void Initialize()
        {
            Debug.Log("[AddressableService] Initialized using real Addressables.");
        }

        public void LoadAssetAsync<T>(string address, Action<T> onComplete) where T : UnityEngine.Object
        {
            UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(address).Completed += handle => onComplete?.Invoke(handle.Result);
        }

        public void InstantiateAsync(string address, Action<GameObject> onComplete)
        {
            UnityEngine.AddressableAssets.Addressables.InstantiateAsync(address).Completed += handle => onComplete?.Invoke(handle.Result);
        }

        public void ReleaseAsset(object asset)
        {
            UnityEngine.AddressableAssets.Addressables.Release(asset);
        }
    }
}
