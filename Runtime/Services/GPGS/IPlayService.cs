using System;
using System.Collections.Generic;
using Speedup.Core;

namespace Speedup.Services.GPGS
{
    public struct PlayServiceLeaderboardEntry
    {
        public int Rank;
        public string Username;
        public long Score;
    }

    public interface IPlayService : IGameService
    {
        void Authenticate(Action<bool> onComplete = null);
        void GetTopScores(string leaderboardId, int count, Action<bool, List<PlayServiceLeaderboardEntry>> onComplete);
        void ShowLeaderboard(string leaderboardId = "");
        void ReportScore(string leaderboardId, long score);
        void UnlockAchievement(string achievementId);
        void SaveDataToCloud(string filename, string jsonData, Action<bool> onComplete = null);
        void LoadDataFromCloud(string filename, Action<bool, string> onComplete = null);
    }
}
