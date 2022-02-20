using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class ChallengeData
    {
        [Serializable]
        public class ChallengeScore
        {
            [field: SerializeField] public int WeekIndex { get; set; } = -1;
            [field: SerializeField] public int Score { get; set; } = 0;
        }

        [Serializable]
        public class ChallengeAttemptData
        {
            [field: SerializeField] public int DayIndex { get; set; } = -1;
            [field: SerializeField] public int AttemptCount { get; set; } = 0;
        }

        [field: SerializeField] public List<ChallengeScore> ChallengeScores { get; set; } = new List<ChallengeScore>();
        [field: SerializeField] public ChallengeAttemptData AttemptData { get; set; } = new ChallengeAttemptData();

        public void ChallengeAttempted()
        {
            AttemptData.AttemptCount++;
            Save(this);
        }

        public void ChallengeScored(int weekIndex, int score)
        {
            ChallengeScore challengeScore = ChallengeScores.FirstOrDefault(p => p.WeekIndex == weekIndex);
            bool requiresSave = false;
            if (challengeScore == null)
            {
                challengeScore = new ChallengeScore
                {
                    WeekIndex = weekIndex,
                    Score = score
                };
                ChallengeScores.Add(challengeScore);
                requiresSave = true;
            }
            else if (challengeScore.Score < score)
            {
                challengeScore.Score = score;
                requiresSave = true;
            }

            if (requiresSave)
            {
                Save(this);
            }
        }

        public static void Save(ChallengeData challengeData)
        {
            string serialized = JsonUtility.ToJson(challengeData);
            PersistentDataHelper.SetString(PersistentDataKeys.ChallengeData, serialized, true);
            CircumDebug.Log($"Saved challenge data: {challengeData}");
        }

        public static ChallengeData Load()
        {
            if (!PlayerPrefs.HasKey(PersistentDataKeys.ChallengeData))
            {
                CircumDebug.Log("Created new challenge data since we can't find saved key");
                return new ChallengeData();
            }
            
            string serializedChallengeData = PersistentDataHelper.GetString(PersistentDataKeys.ChallengeData);
            ChallengeData deserializedChallengeData = JsonUtility.FromJson<ChallengeData>(serializedChallengeData);
            CircumDebug.Log($"Loaded challenge data: {deserializedChallengeData}");

            if (deserializedChallengeData == null)
            {
                CircumDebug.Log("No challenge data found, creating new");
                return new ChallengeData();
            }
            
            return deserializedChallengeData;
        }

        public void ResetChallengeAttempts()
        {
            AttemptData.AttemptCount = 0;
            Save(this);
        }
    }
}