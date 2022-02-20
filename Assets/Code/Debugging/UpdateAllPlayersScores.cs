using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Level;
using Code.Level.Player;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

#if ENABLE_PLAYFABADMIN_API
using PlayFab.AdminModels;
using GetUserDataRequest = PlayFab.AdminModels.GetUserDataRequest;
using UserDataRecord = PlayFab.AdminModels.UserDataRecord;
#endif

namespace Code.Debugging
{
    public class UpdateAllPlayersScores : MonoBehaviour
    {
        private const string AllPlayersSegmentId = "3F4C422E400D38D0";
        private const string LevelStatsKeyStartsWith = "Circum_LevelStats_";

        [SerializeField] private LevelProgression _levelProgression;

#if ENABLE_PLAYFABADMIN_API
        [ContextMenu(nameof(StartScoresUpdate))]
        private void StartScoresUpdate()
        {
            PlayFabAdminAPI.GetPlayersInSegment(new GetPlayersInSegmentRequest
            {
                SegmentId = AllPlayersSegmentId
            }, result =>
            {
                Debug.Log($"Running scores update for {result.PlayerProfiles.Count} players");
                StartCoroutine(UpdateScoreForAllPlayers(result.PlayerProfiles));
            }, error => Debug.LogError(error.ErrorMessage));
        }

        private IEnumerator UpdateScoreForAllPlayers(List<PlayerProfile> resultPlayerProfiles)
        {
            foreach (PlayerProfile playerProfile in resultPlayerProfiles)
            {
                yield return UpdateScoresForPlayer(playerProfile.PlayerId);
            }
        }

        private IEnumerator UpdateScoresForPlayer(string playerId)
        {
            bool hasCompletedRequest = false;
            PlayFabAdminAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = playerId
            }, result =>
            {
                Debug.Log($"Got {result.Data.Count} data items from player {playerId}");
                int totalScore = 0;
                
                foreach (KeyValuePair<string, UserDataRecord> userData in result.Data)
                {
                    if (!userData.Key.StartsWith(LevelStatsKeyStartsWith))
                    {
                        continue;
                    }

                    string levelName = userData.Key.Replace(LevelStatsKeyStartsWith, "");
                    LevelLayout levelLayout = _levelProgression.LevelLayout.FirstOrDefault(p => p.name.Equals(levelName));
                    
                    if (levelLayout == null)
                    {
                        Debug.LogError($"No level layout with name {levelName}");
                        continue;
                    }

                    string serializedLevelMetaData = userData.Value.Value;
                    LevelMetaData levelMetaData = JsonUtility.FromJson<LevelMetaData>(serializedLevelMetaData);

                    int levelScore = PlayerScoreHelper.GetScoreFromLevel(levelLayout.FullMarksTime, levelMetaData.LevelTime, levelMetaData.HasPerfectTime, PlayerScoreHelper.MaxScoreForLevel);
                    totalScore += levelScore;
                }

                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "updatePlayerStatistic",
                    FunctionParameter = new {PlayerId = playerId, StatisticName = PersistentDataKeys.TotalScoresStatistic, StatisticValue = totalScore}
                }, scriptResult =>
                {
                    Debug.Log($"Successfully updated player {playerId} score to {totalScore}");
                    hasCompletedRequest = true;
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
#endif
    }
}