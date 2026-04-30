#pragma warning disable CS0618 // Unity's Social API is deprecated, but still needed here until migrated to Apple's GameKit package.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
//using UnityEngine.SocialPlatforms.GameCenter;
using Speedup.Services.GPGS; // Re-using the DTO struct from the original play service

namespace Speedup.Services.Apple
{
    public class AppleLeaderboard
    {
        public void ReportScore(string leaderboardId, long score)
        {
            if (!Social.localUser.authenticated)
            {
                Debug.LogWarning($"[AppleLeaderboard] User not authenticated. Cannot report score to {leaderboardId}.");
                return;
            }

            Social.ReportScore(score, leaderboardId, (success) =>
            {
                if (success)
                    Debug.Log($"[AppleLeaderboard] Successfully reported score {score} to {leaderboardId}.");
                else
                    Debug.LogError($"[AppleLeaderboard] Failed to report score to {leaderboardId}.");
            });
        }

        public void ShowLeaderboardUI(string leaderboardId = "")
        {
            if (!Social.localUser.authenticated)
            {
                Debug.LogWarning("[AppleLeaderboard] User not authenticated. Cannot show leaderboard UI.");
                return;
            }

            // Game Center natively supports opening a specific leaderboard if you configure it 
            // via the GameCenterPlatform extension methods, but Social.ShowLeaderboardUI defaults to all.
            Social.ShowLeaderboardUI();
        }

        public void GetTopScores(string leaderboardId, int count, Action<bool, List<PlayServiceLeaderboardEntry>> onComplete)
        {
            if (!Social.localUser.authenticated)
            {
                Debug.LogWarning($"[AppleLeaderboard] User not authenticated. Cannot get top scores for {leaderboardId}.");
                onComplete?.Invoke(false, null);
                return;
            }

            ILeaderboard leaderboard = Social.CreateLeaderboard();
            leaderboard.id = leaderboardId;
            leaderboard.range = new UnityEngine.SocialPlatforms.Range(1, count);
            leaderboard.timeScope = TimeScope.AllTime;

            leaderboard.LoadScores((success) =>
            {
                if (success && leaderboard.scores != null)
                {
                    var list = new List<PlayServiceLeaderboardEntry>();
                    foreach (IScore score in leaderboard.scores)
                    {
                        list.Add(new PlayServiceLeaderboardEntry
                        {
                            Rank = score.rank,
                            // userID in Apple Game Center resolves to an alias automatically in some versions,
                            // or it might be raw ID. If more context is needed, LoadUsers can be called on the Social API.
                            Username = score.userID, 
                            Score = score.value
                        });
                    }
                    onComplete?.Invoke(true, list);
                }
                else
                {
                    Debug.LogError($"[AppleLeaderboard] Failed to load top scores for {leaderboardId}.");
                    onComplete?.Invoke(false, null);
                }
            });
        }
    }
}
