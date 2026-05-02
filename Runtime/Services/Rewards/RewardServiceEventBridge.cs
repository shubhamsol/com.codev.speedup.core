using UnityEngine;
using Speedup.Core;

namespace Speedup.Services.Rewards
{
    /// <summary>
    /// Inspector-friendly listener target. Attach to any object and wire
    /// RewardClickEmitter.OnRewardClicked -> ClaimReward in UI events.
    /// </summary>
    public class RewardServiceEventBridge : MonoBehaviour
    {
        public void ClaimReward(RewardPayload payload)
        {
            IRewardService rewardService = ServiceLocator.Get<IRewardService>();
            if (rewardService == null)
            {
                Debug.LogWarning("[RewardServiceEventBridge] IRewardService not registered.");
                return;
            }

            rewardService.TryClaimReward(payload);
        }
    }
}
