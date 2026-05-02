using System;
using Speedup.Core;

namespace Speedup.Services.Rewards
{
    /// <summary>
    /// Resolves reward payloads into the proper backend service (economy/inventory/etc.).
    /// </summary>
    public interface IRewardService : IGameService
    {
        event Action<RewardPayload> OnRewardClaimed;
        event Action<RewardPayload, string> OnRewardClaimFailed;

        bool TryClaimReward(RewardPayload payload);
    }
}
