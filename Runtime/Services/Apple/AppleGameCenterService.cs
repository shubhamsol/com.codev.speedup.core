#pragma warning disable CS0618 // Unity's Social API is deprecated, but still needed here until migrated to Apple's GameKit package.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Speedup.Core;
using Speedup.Services.GPGS; // Reusing the IPlayService and PlayServiceLeaderboardEntry

namespace Speedup.Services.Apple
{
    /// <summary>
    /// Wrapper for Apple Game Center using Unity's built-in Social API.
    /// Orchestrates AppleCloudSave and AppleLeaderboard instances for clean separation of concerns.
    /// </summary>
    public class AppleGameCenterService : IPlayService
    {
        private AppleCloudSave _cloudSave;
        private AppleLeaderboard _leaderboard;

        public void Initialize()
        {
            _cloudSave = new AppleCloudSave();
            _leaderboard = new AppleLeaderboard();

            // Unity's iOS backend will automatically use GameCenterPlatform
            Debug.Log("[AppleGameCenterService] Initialized Apple Game Center Service.");
        }

        public void Authenticate(Action<bool> onComplete = null)
        {
            Debug.Log("[AppleGameCenterService] Authenticating...");
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                    Debug.Log($"[AppleGameCenterService] Authenticated successfully as {Social.localUser.userName}");
                else
                    Debug.LogError("[AppleGameCenterService] Authentication failed.");

                onComplete?.Invoke(success);
            });
        }

        public void ShowLeaderboard(string leaderboardId = "")
        {
            _leaderboard.ShowLeaderboardUI(leaderboardId);
        }

        public void GetTopScores(string leaderboardId, int count, Action<bool, List<PlayServiceLeaderboardEntry>> onComplete)
        {
            _leaderboard.GetTopScores(leaderboardId, count, onComplete);
        }

        public void ReportScore(string leaderboardId, long score)
        {
            _leaderboard.ReportScore(leaderboardId, score);
        }

        public void UnlockAchievement(string achievementId)
        {
            if (!Social.localUser.authenticated)
            {
                Debug.LogWarning($"[AppleGameCenterService] User not authenticated. Cannot unlock achievement {achievementId}");
                return;
            }

            Social.ReportProgress(achievementId, 100.0d, (success) =>
            {
                if (success)
                    Debug.Log($"[AppleGameCenterService] Successfully unlocked achievement {achievementId}");
                else
                    Debug.LogError($"[AppleGameCenterService] Failed to unlock achievement {achievementId}");
            });
        }

        public void SaveDataToCloud(string filename, string jsonData, Action<bool> onComplete = null)
        {
            _cloudSave.SaveToCloud(filename, jsonData, onComplete);
        }

        public void LoadDataFromCloud(string filename, Action<bool, string> onComplete = null)
        {
            _cloudSave.LoadFromCloud(filename, onComplete);
        }
    }
}
