using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Level.Player;
using Code.UI;
using PlayFab;
using UnityEngine;
using UnityExtras.Core;
using UpdateUserDataRequest = PlayFab.AdminModels.UpdateUserDataRequest;

#if ENABLE_PLAYFABADMIN_API
using PlayFab.AdminModels;
using GetUserDataRequest = PlayFab.AdminModels.GetUserDataRequest;
using UserDataRecord = PlayFab.AdminModels.UserDataRecord;
#endif

namespace Code.Debugging
{
    public class UpdateAllLeaderboards : MonoBehaviour
    {
        private const int MaxLeaderboardEntries = 5;
        private const string LeaderboardPlayerId = "2899D85EEB2F6F0D";
        private const string AllPlayersSegmentId = "3F4C422E400D38D0";
        private const string LevelStatsKeyStartsWith = "Circum_LevelStats_";
        private const string LeaderboardKeyEndsWith = "_GlobalBest";

        [Serializable]
        private class LevelLeaderboardResult
        {
            public PlayerLevelData[] Data = new PlayerLevelData[0];
        }

#if ENABLE_PLAYFABADMIN_API

        [ContextMenu(nameof(StartLeaderboardUpdate))]
        private void StartLeaderboardUpdate()
        {
            PlayFabAdminAPI.GetPlayersInSegment(new GetPlayersInSegmentRequest
            {
                SegmentId = AllPlayersSegmentId
            }, result =>
            {
                Debug.Log($"Running leaderboard update for {result.PlayerProfiles.Count} players");
                StartCoroutine(UpdateLeaderboardForAllPlayers(result.PlayerProfiles));
            }, error => Debug.LogError(error.ErrorMessage));
        }

        private IEnumerator UpdateLeaderboardForAllPlayers(List<PlayerProfile> playerProfiles)
        {
            for (int index = 0; index < playerProfiles.Count; index++)
            {
                PlayerProfile profile = playerProfiles[index];
                Debug.Log($"Starting leaderboard update for player {profile.DisplayName} ({index + 1}/{playerProfiles.Count})");
                yield return UpdateLeaderboardForPlayer(profile.PlayerId, profile.DisplayName);
            }
            Debug.Log("--- Leaderboard update completed! ---");
        }

        private IEnumerator UpdateLeaderboardForPlayer(string playerId, string displayName)
        {
            bool hasCompletedRequest = false;
            PlayFabAdminAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = LeaderboardPlayerId,
            }, leaderboardDataResult =>
            {
                PlayFabAdminAPI.GetUserData(new GetUserDataRequest
                {
                    PlayFabId = playerId
                }, playerDataResult =>
                {
                    Dictionary<string, string> leaderboardUpdates = new Dictionary<string, string>();

                    foreach (KeyValuePair<string, UserDataRecord> userData in playerDataResult.Data)
                    {
                        if (!userData.Key.StartsWith(LevelStatsKeyStartsWith))
                        {
                            continue;
                        }

                        string levelName = userData.Key.Replace(LevelStatsKeyStartsWith, "");
                        string leaderboardKeyName = $"{levelName}{LeaderboardKeyEndsWith}";

                        LevelLeaderboardResult leaderboardData;
                        if (!leaderboardDataResult.Data.TryGetValue(leaderboardKeyName, out var leaderboardDataForLevel))
                        {
                            Debug.Log($"Could not find leaderboard data for level {levelName}. Creating new");
                            leaderboardData = new LevelLeaderboardResult();
                        }
                        else
                        {
                            var serializedLevelLeaderboardData = leaderboardDataForLevel.Value;
                            leaderboardData = JsonUtility.FromJson<LevelLeaderboardResult>(serializedLevelLeaderboardData);
                        }

                        if (leaderboardData.Data.Any(p => p.PlayfabId.Equals(playerId)))
                        {
                            // user already in leaderboard
                            continue;
                        }

                        string serializedPlayerLevelData = userData.Value.Value;
                        LevelMetaData playerLevelData = JsonUtility.FromJson<LevelMetaData>(serializedPlayerLevelData);

                        leaderboardData.Data = leaderboardData.Data.Append(new PlayerLevelData(playerId, displayName, playerLevelData.LevelTime, playerLevelData.HasPerfectTime)).ToArray();
                        SortLevelLeaderboard(ref leaderboardData);

                        bool playerIsNowInLeaderboard = leaderboardData.Data.Any(p => p.PlayfabId.Equals(playerId));
                        if (playerIsNowInLeaderboard)
                        {
                            leaderboardUpdates.Add(leaderboardKeyName, JsonUtility.ToJson(leaderboardData));
                        }
                    }

                    if (leaderboardUpdates.Count > 0)
                    {
                        UpdateLeaderboardData(leaderboardUpdates, () =>
                        {
                            Debug.Log($"Updated leaderboard for player {displayName} : \n{leaderboardUpdates.ToArray().JoinToString("\n")}");
                            hasCompletedRequest = true;
                        });
                    }
                    else
                    {
                        Debug.Log($"Leaderboards didn't need changing for player {displayName}");
                        hasCompletedRequest = true;
                    }
                }, error =>
                {
                    Debug.LogError(error);
                    hasCompletedRequest = true;
                });
            }, error =>
            {
                Debug.LogError(error);
                hasCompletedRequest = true;
            });

            yield return new WaitUntil(() => hasCompletedRequest);
        }

        private void UpdateLeaderboardData(Dictionary<string, string> titleData, Action onComplete)
        {
            if (titleData.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            Dictionary<string, string> updateNow = titleData.Take(10).ToDictionary(p => p.Key, q => q.Value);
            Dictionary<string, string> updateNext = titleData.Skip(10).ToDictionary(p => p.Key, q => q.Value);

            PlayFabAdminAPI.UpdateUserData(new UpdateUserDataRequest
            {
                PlayFabId = LeaderboardPlayerId,
                Data = updateNow
            }, result =>
            {
                UpdateLeaderboardData(updateNext, onComplete);
            }, error =>
            {
                Debug.LogError($"Could not update leaderboard : {error}");
                onComplete?.Invoke();
            });
        }

        private void SortLevelLeaderboard(ref LevelLeaderboardResult original)
        {
            original.Data = original.Data.OrderByDescending(data => (data.IsPerfect ? 100000f : 0f) - data.Time).Take(MaxLeaderboardEntries).ToArray();
        }

#endif
    }
}