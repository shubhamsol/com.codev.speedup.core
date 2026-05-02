using System;

namespace Speedup.Services.Rewards
{
    /// <summary>
    /// Generic reward descriptor that can be emitted from UI and resolved by services.
    /// </summary>
    [Serializable]
    public struct RewardPayload
    {
        public RewardType Type;
        public string Id;
        public int Amount;
    }
}
