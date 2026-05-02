using UnityEngine;
using Speedup.Core;
using Speedup.Services.Economy;
using Speedup.Services.Inventory;
using System;

namespace Speedup.Services.Rewards
{
    /// <summary>
    /// Default resolver implementation for reward payloads.
    /// </summary>
    public class RewardService : IRewardService
    {
        public event Action<RewardPayload> OnRewardClaimed;
        public event Action<RewardPayload, string> OnRewardClaimFailed;

        public void Initialize()
        {
        }

        public bool TryClaimReward(RewardPayload payload)
        {
            if (payload.Type == RewardType.None)
            {
                return Fail(payload, "Reward type is None.");
            }

            if (string.IsNullOrWhiteSpace(payload.Id))
            {
                return Fail(payload, "Reward Id is empty.");
            }

            if (payload.Amount <= 0)
            {
                return Fail(payload, "Reward amount must be greater than 0.");
            }

            switch (payload.Type)
            {
                case RewardType.Currency:
                {
                    IEconomyService economy = ServiceLocator.Get<IEconomyService>();
                    if (economy == null)
                    {
                        return Fail(payload, "IEconomyService not registered.");
                    }

                    economy.AddCurrency(payload.Id, payload.Amount);
                    break;
                }
                case RewardType.InventoryItem:
                {
                    IInventoryService inventory = ServiceLocator.Get<IInventoryService>();
                    if (inventory == null)
                    {
                        return Fail(payload, "IInventoryService not registered.");
                    }

                    inventory.AddItem(payload.Id, payload.Amount);
                    break;
                }
                default:
                    return Fail(payload, $"Unsupported reward type: {payload.Type}");
            }

            OnRewardClaimed?.Invoke(payload);
            return true;
        }

        private bool Fail(RewardPayload payload, string reason)
        {
            Debug.LogWarning($"[RewardService] Failed to claim reward ({payload.Type}:{payload.Id} x{payload.Amount}) - {reason}");
            OnRewardClaimFailed?.Invoke(payload, reason);
            return false;
        }
    }
}
