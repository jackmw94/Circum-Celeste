using System;
using System.Collections.Generic;
using Code.Core;
using Code.Debugging;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create ChallengeLevel", fileName = "ChallengeLevel", order = 0)]
    public class ChallengeLevel : ScriptableObject
    {
        /// <summary>
        /// Class containing json representations of challenge level data
        /// Useful to avoid nested prefab serialization issues
        /// </summary>
        [Serializable]
        public class ChallengeLevelProxy
        {
            [field: SerializeField] public string SerializedLevelLayout { get; set; }
            [field: SerializeField] public string SerializedChallengeConfiguration { get; set; }
        }

        [Serializable]
        public class ChallengeLevelConfiguration
        {
            [field: SerializeField] public int DailyAttempts { get; set; } = 3;
            [field: SerializeField] public int Points { get; set; }
            [field: SerializeField] public int WeekIndex { get; set; }
            [field: SerializeField, HideInInspector] public string LevelName { get; set; }
        }
        
        [SerializeField] private LevelLayout _levelLayout;
        [SerializeField] private ChallengeLevelConfiguration _challengeConfiguration;

        public LevelLayout LevelLayout => _levelLayout;
        public int Points => _challengeConfiguration.Points;
        public string LevelName => _challengeConfiguration.LevelName;
        public int WeekIndex => _challengeConfiguration.WeekIndex;
        public int AttemptsRemaining(int attemptsUsed) => _challengeConfiguration.DailyAttempts - attemptsUsed;
        
        public static void RequestChallengeLevel(int weekIndex, Action<ChallengeLevel> challengeLevelCallback)
        {
            string challengeKey = PersistentDataKeys.ChallengeName(weekIndex);
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest
            {
                Keys = new List<string>
                {
                    challengeKey
                }
            }, result =>
            {
                string serializedChallengeLevelProxy = result.Data[challengeKey];
                ChallengeLevelProxy challengeLevelProxy = JsonUtility.FromJson<ChallengeLevelProxy>(serializedChallengeLevelProxy);
                
                ChallengeLevel challengeLevel = CreateInstance<ChallengeLevel>();
                LevelLayout levelLayout = CreateInstance<LevelLayout>();

                JsonUtility.FromJsonOverwrite(challengeLevelProxy.SerializedLevelLayout, levelLayout);
                ChallengeLevelConfiguration configuration = JsonUtility.FromJson<ChallengeLevelConfiguration>(challengeLevelProxy.SerializedChallengeConfiguration);
                
                levelLayout.name = configuration.LevelName;
                challengeLevel._levelLayout = levelLayout;
                challengeLevel._challengeConfiguration = configuration;

                challengeLevelCallback(challengeLevel);
            }, error =>
            {
                CircumDebug.LogError(error.ErrorMessage);
                challengeLevelCallback(null);
            });
        }

        public ChallengeLevelProxy GetChallengeLevelProxy()
        {
            _challengeConfiguration.LevelName = _levelLayout.name;
            return new ChallengeLevelProxy
            {
                SerializedChallengeConfiguration = JsonUtility.ToJson(_challengeConfiguration),
                SerializedLevelLayout = JsonUtility.ToJson(_levelLayout)
            };
        }
    }
}