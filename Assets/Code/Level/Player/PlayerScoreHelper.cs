using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Level.Player
{
    public static class PlayerScoreHelper
    {
        public const int MaxScoreForLevel = 100;

        [Serializable]
        public class PlayerScore
        {
            [Serializable]
            public class LevelScore
            {
                [SerializeField] private string _levelName = "<level name>";
                [SerializeField] private int _score = 0;

                public LevelScore(string levelName, int score)
                {
                    _levelName = levelName;
                    _score = score;
                }

                public string LevelName
                {
                    get => _levelName;
                    set => _levelName = value;
                }

                public int Score
                {
                    get => _score;
                    set => _score = value;
                }
            }
            
            [SerializeField] private List<LevelScore> _levelScores = new List<LevelScore>();
            [SerializeField] private int _totalScore = 0;

            public List<LevelScore> LevelScores
            {
                get => _levelScores;
                set => _levelScores = value;
            }

            public int TotalScore
            {
                get => _totalScore;
                set => _totalScore = value;
            }
        }

        public static PlayerScore GetPlayerScore(Dictionary<string,LevelLayout> levelLayouts, Dictionary<string, LevelStats> levelStats, ChallengeData challengeData, int currentWeekIndex)
        {
            PlayerScore playerScores = new PlayerScore();
            
            foreach (KeyValuePair<string,LevelStats> statsForLevel in levelStats)
            {
                string levelName = statsForLevel.Key;
                
                if (!levelLayouts.TryGetValue(levelName, out LevelLayout levelLayout))
                {
                    continue;
                }

                if (!levelLayout.ContributesToScoring)
                {
                    continue;
                }
                
                LevelRecording levelRecording = statsForLevel.Value.LevelRecording;
                float levelTime = levelRecording.RecordingData.LevelTime;
                float fullMarksTime = levelLayout.FullMarksTime;
                bool wasPerfect = levelRecording.RecordingData.IsPerfect;

                int levelScore = GetScoreFromLevel(fullMarksTime, levelTime, wasPerfect, MaxScoreForLevel);
                playerScores.LevelScores.Add(new PlayerScore.LevelScore(levelName, levelScore));
                playerScores.TotalScore += levelScore;
            }

            foreach (ChallengeData.ChallengeScore challengeScore in challengeData.ChallengeScores)
            {
                int weeksSince = currentWeekIndex - challengeScore.WeekIndex;
                if (weeksSince < 12)
                {
                    playerScores.TotalScore += challengeScore.Score;
                }
            }

            return playerScores;
        }
        
        public static int GetScoreFromLevel(float fullMarksTime, float recordingTime, bool isPerfect, int maxScoreForLevel)
        {
            float timeScoreFactor = Mathf.Clamp01(fullMarksTime / recordingTime);
            int levelScore = Mathf.FloorToInt(timeScoreFactor * maxScoreForLevel * (isPerfect ? 1f : 0.5f));
            return levelScore;
        }
    }
}