using System;
using UnityEngine;
using UnityEngine.Events;
using Speedup.Core;

namespace Speedup.Services.Rewards
{
    [Serializable]
    public class RewardPayloadEvent : UnityEvent<RewardPayload>
    {
    }

    /// <summary>
    /// UI helper to emit a configured reward payload when a button is clicked.
    /// Designers can wire OnRewardClicked listeners in the Inspector.
    /// </summary>
    public class RewardClickEmitter : MonoBehaviour
    {
        [Header("Reward Source")]
        [Tooltip("If assigned, this RewardDefinition is used. Otherwise Configured Reward payload below is used.")]
        [SerializeField] private RewardDefinition rewardDefinition;

        [Header("Configured Reward")]
        [SerializeField] private RewardPayload rewardPayload;

        [Header("Dispatch")]
        [Tooltip("If true, also claims reward through IRewardService after firing OnRewardClicked.")]
        [SerializeField] private bool autoClaimViaService;

        [SerializeField] private RewardPayloadEvent onRewardClicked = new RewardPayloadEvent();

        public RewardPayloadEvent OnRewardClicked => onRewardClicked;

        public RewardPayload RewardPayload
        {
            get => rewardPayload;
            set => rewardPayload = value;
        }

        /// <summary>
        /// Assign this method to UI Button.onClick.
        /// </summary>
        public void EmitConfiguredReward()
        {
            RewardPayload resolvedPayload = GetResolvedPayload();
            onRewardClicked?.Invoke(resolvedPayload);

            if (!autoClaimViaService)
            {
                return;
            }

            IRewardService rewardService = ServiceLocator.Get<IRewardService>();
            if (rewardService == null)
            {
                Debug.LogWarning("[RewardClickEmitter] IRewardService not registered.");
                return;
            }

            rewardService.TryClaimReward(resolvedPayload);
        }

        private RewardPayload GetResolvedPayload()
        {
            return rewardDefinition != null ? rewardDefinition.ToPayload() : rewardPayload;
        }
    }
}
