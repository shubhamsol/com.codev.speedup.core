using UnityEngine;

namespace Speedup.Services.Rewards
{
    /// <summary>
    /// Designer-friendly reward asset.
    /// Create once and reuse across shop, daily rewards, missions, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "Reward_", menuName = "Speedup/Rewards/Reward Definition")]
    public class RewardDefinition : ScriptableObject
    {
        [SerializeField] private RewardType rewardType = RewardType.Currency;
        [SerializeField] private string rewardId;
        [SerializeField] private int amount = 1;

        public RewardType RewardType => rewardType;
        public string RewardId => rewardId;
        public int Amount => amount;

        public RewardPayload ToPayload()
        {
            return new RewardPayload
            {
                Type = rewardType,
                Id = rewardId,
                Amount = amount
            };
        }
    }
}
