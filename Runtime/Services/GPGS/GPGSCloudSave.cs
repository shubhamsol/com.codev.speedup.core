using System;
using System.Text;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

namespace Speedup.Services.GPGS
{
    public class GPGSCloudSave
    {
        public void SaveToCloud(string filename, string jsonData, Action<bool> onComplete)
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                Debug.LogWarning("[GPGSCloudSave] User not authenticated. Cannot save.");
                onComplete?.Invoke(false);
                return;
            }

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, (status, metadata) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(jsonData);
                        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
                        savedGameClient.CommitUpdate(metadata, update, data, (commitStatus, commitMetadata) =>
                        {
                            bool success = (commitStatus == SavedGameRequestStatus.Success);
                            if (success)
                                Debug.Log($"[GPGSCloudSave] Successfully saved {filename}");
                            else
                                Debug.LogError($"[GPGSCloudSave] Failed to commit {filename}. Status: {commitStatus}");
                            
                            onComplete?.Invoke(success);
                        });
                    }
                    else
                    {
                        Debug.LogError($"[GPGSCloudSave] Failed to open {filename} for saving. Status: {status}");
                        onComplete?.Invoke(false);
                    }
                });
        }

        public void LoadFromCloud(string filename, Action<bool, string> onComplete)
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                Debug.LogWarning("[GPGSCloudSave] User not authenticated. Cannot load.");
                onComplete?.Invoke(false, null);
                return;
            }

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, (status, metadata) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        savedGameClient.ReadBinaryData(metadata, (readStatus, data) =>
                        {
                            if (readStatus == SavedGameRequestStatus.Success)
                            {
                                string jsonData = Encoding.UTF8.GetString(data);
                                Debug.Log($"[GPGSCloudSave] Successfully loaded {filename}");
                                onComplete?.Invoke(true, jsonData);
                            }
                            else
                            {
                                Debug.LogError($"[GPGSCloudSave] Failed to read binary data for {filename}. Status: {readStatus}");
                                onComplete?.Invoke(false, null);
                            }
                        });
                    }
                    else
                    {
                        Debug.LogError($"[GPGSCloudSave] Failed to open {filename} for loading. Status: {status}");
                        onComplete?.Invoke(false, null);
                    }
                });
        }
    }
}
