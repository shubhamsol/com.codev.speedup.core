using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Speedup.Core;

namespace Speedup.Services.GPGS
{
    /// <summary>
    /// Wrapper for Google Play Games Services using concrete API calls.
    /// Orchestrates GPGSCloudSave and GPGSLeaderboard instances for clean separation of concerns.
    /// </summary>
    public class PlayGamesService : IPlayService
    {
        private GPGSCloudSave _cloudSave;
        private GPGSLeaderboard _leaderboard;

        public void Initialize()
        {
            _cloudSave = new GPGSCloudSave();
            _leaderboard = new GPGSLeaderboard();

            // Setup Play Games configuration for GPGS v11+ (v2 API)
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            Debug.Log("[PlayGamesService] Initialized Google Play Games.");
        }

        public void Authenticate(Action<bool> onComplete = null)
        {
            Debug.Log("[PlayGamesService] Authenticating...");
            PlayGamesPlatform.Instance.Authenticate((SignInStatus status) =>
            {
                bool success = (status == SignInStatus.Success);
                if (success)
                    Debug.Log($"[PlayGamesService] Authenticated successfully as {PlayGamesPlatform.Instance.localUser.userName}");
                else
                    Debug.LogError($"[PlayGamesService] Authentication failed with status: {status}");

                onComplete?.Invoke(success);
            });
        }

        public void ShowLeaderboard(string leaderboardId = "")
        {
            _leaderboard.ShowLeaderboardUI(leaderboardId);
        }

        public void GetTopScores(string leaderboardId, int count, System.Action<bool, System.Collections.Generic.List<PlayServiceLeaderboardEntry>> onComplete)
        {
            _leaderboard.GetTopScores(leaderboardId, count, onComplete);
        }

        public void ReportScore(string leaderboardId, long score)
        {
            _leaderboard.ReportScore(leaderboardId, score);
        }

        public void UnlockAchievement(string achievementId)
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                Debug.LogWarning($"[PlayGamesService] User not authenticated. Cannot unlock achievement {achievementId}");
                return;
            }

            PlayGamesPlatform.Instance.ReportProgress(achievementId, 100.0f, (success) =>
            {
                if (success)
                    Debug.Log($"[PlayGamesService] Successfully unlocked achievement {achievementId}");
                else
                    Debug.LogError($"[PlayGamesService] Failed to unlock achievement {achievementId}");
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
