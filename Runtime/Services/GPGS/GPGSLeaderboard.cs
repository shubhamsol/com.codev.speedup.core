using System;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Speedup.Services.GPGS
{
    public class GPGSLeaderboard
    {
        public void GetTopScores(string leaderboardId, int count, Action<bool, List<PlayServiceLeaderboardEntry>> onComplete)
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                Debug.LogWarning($"[GPGSLeaderboard] User not authenticated. Cannot get top scores for {leaderboardId}.");
                onComplete?.Invoke(false, null);
                return;
            }

            PlayGamesPlatform.Instance.LoadScores(
                leaderboardId,
                LeaderboardStart.TopScores,
                count,
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.AllTime,
                (LeaderboardScoreData data) =>
                {
                    if (data != null && data.Valid)
                    {
                        var list = new List<PlayServiceLeaderboardEntry>();
                        foreach (var score in data.Scores)
                        {
                            list.Add(new PlayServiceLeaderboardEntry
                            {
                                Rank = score.rank,
                                Username = score.userID,
                                Score = score.value
                            });
                        }
                        onComplete?.Invoke(true, list);
                    }
                    else
                    {
                        Debug.LogError($"[GPGSLeaderboard] Failed to load top scores for {leaderboardId}.");
                        onComplete?.Invoke(false, null);
                    }
                });
        }

        public void ReportScore(string leaderboardId, long score)
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                Debug.LogWarning($"[GPGSLeaderboard] User not authenticated. Cannot report score to {leaderboardId}.");
                return;
            }

            PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, (success) =>
            {
                if (success)
                    Debug.Log($"[GPGSLeaderboard] Successfully reported score {score} to {leaderboardId}.");
                else
                    Debug.LogError($"[GPGSLeaderboard] Failed to report score to {leaderboardId}.");
            });
        }

        public void ShowLeaderboardUI(string leaderboardId = "")
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                Debug.LogWarning("[GPGSLeaderboard] User not authenticated. Cannot show leaderboard UI.");
                return;
            }

            if (string.IsNullOrEmpty(leaderboardId))
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI();
            }
            else
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardId);
            }
        }
    }
}
