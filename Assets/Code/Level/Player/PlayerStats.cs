using System;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class PlayerStats
    {
        [SerializeField] private bool _completedTutorials;
        [SerializeField] private RunTracker _runTracker = null;

        [SerializeField] private int _currentLevelIndex = 0;
        [SerializeField] private int _highestLevelReachedIndex = 0;
        [SerializeField] private int _highestNoDeathLevelReachedIndex = 0;
        [SerializeField] private int _highestPerfectLevelReachedIndex = 0;

        public RunTracker RunTracker => _runTracker;
        public bool CompletedTutorials => _completedTutorials;
        public int HighestLevelIndex => _highestLevelReachedIndex;
        public int HighestLevelNoDeathsIndex => _highestNoDeathLevelReachedIndex;
        public int HighestPerfectLevelIndex => _highestPerfectLevelReachedIndex;
        public int RestartLevelIndex => _currentLevelIndex;

        public bool IsNextLevelUnlocked(int currentLevelIndex) => currentLevelIndex < _highestLevelReachedIndex;

        public void UpdateHighestLevel(int levelIndex, bool noDeaths, bool noHits, bool hasSkipped, out bool firstTimeCompletingLevel)
        {
            int prevHighestLevel = _highestLevelReachedIndex;
            _highestLevelReachedIndex = Mathf.Max(levelIndex + 1, _highestLevelReachedIndex);

            firstTimeCompletingLevel = prevHighestLevel != _highestLevelReachedIndex;
            
            if (hasSkipped)
            {
                return;
            }

            if (noDeaths)
            {
                _highestNoDeathLevelReachedIndex = Mathf.Max(levelIndex, _highestNoDeathLevelReachedIndex);
            }
            
            if (noHits)
            {
                _highestPerfectLevelReachedIndex = Mathf.Max(levelIndex, _highestPerfectLevelReachedIndex);
            }
        }

        public void SetCurrentLevel(int currentLevelIndex)
        {
            _currentLevelIndex = currentLevelIndex;
        }

        public void UpdateCompletedTutorials(bool hasCompletedTutorials, bool forceSet = false)
        {
            if (forceSet)
            {
                _completedTutorials = hasCompletedTutorials;
            }
            else
            {
                _completedTutorials |= hasCompletedTutorials;
            }
        }
        
        public override string ToString()
        {
            return $"Tutorials complete = {_completedTutorials}\nHighest level reached = {_highestLevelReachedIndex}\nHighest level with no deaths = {_highestNoDeathLevelReachedIndex}\nHighest level on perfect run = {_highestPerfectLevelReachedIndex}\nLast run = {_runTracker}";
        }

        public static void Save(PlayerStats stats)
        {
            string serialized = JsonUtility.ToJson(stats);
            string compressed = serialized.Compress();

            PersistentDataHelper.SetString(PersistentDataKeys.PlayerStats, compressed, true);
            CircumDebug.Log($"Saved player stats: {stats}");
        }
        
        public static PlayerStats Load()
        {
            if (!PlayerPrefs.HasKey(PersistentDataKeys.PlayerStats))
            {
                CircumDebug.Log("Created new player stats since we can't find saved key");
                return CreateEmptyPlayerStats();
            }
            
            string serializedPlayerStats = PersistentDataHelper.GetString(PersistentDataKeys.PlayerStats);

            if (!string.IsNullOrEmpty(serializedPlayerStats))
            {
                string firstChar = serializedPlayerStats.Substring(0, 1);
                string lastChar = serializedPlayerStats.Substring(serializedPlayerStats.Length - 1, 1);
                if (!firstChar.Equals("{") || !lastChar.Equals("}"))
                {
                    serializedPlayerStats = serializedPlayerStats.Decompress();
                }
            }
            
            PlayerStats deserializedPlayerStats = JsonUtility.FromJson<PlayerStats>(serializedPlayerStats);
            CircumDebug.Log($"Loaded player stats: {deserializedPlayerStats}");

            if (deserializedPlayerStats == null)
            {
                CircumDebug.Log("No player stats found, creating new");
                return CreateEmptyPlayerStats();
            }
            
            deserializedPlayerStats._runTracker ??= new RunTracker();
            return deserializedPlayerStats;
        }

        public static PlayerStats CreateEmptyPlayerStats()
        {
            return new PlayerStats
            {
                _runTracker = new RunTracker()
            };
        }
        
        public static PlayerStats CreateStarterPlayerStats()
        {
            return new PlayerStats
            {
                _completedTutorials = true,
                _runTracker = new RunTracker()
            };
        }
        
        public static PlayerStats CreatePerfectPlayerStats()
        {
            return new PlayerStats
            {
                _completedTutorials = true,
                _highestLevelReachedIndex = int.MaxValue,
                _highestPerfectLevelReachedIndex = int.MaxValue,
                _highestNoDeathLevelReachedIndex = int.MaxValue,
                _runTracker = new RunTracker()
            };
        }

        /// <summary>
        /// Starter stats are empty but have tutorials completed flag set to true
        /// </summary>
        public static void SetStarterPlayerStats()
        {
            Save(CreateStarterPlayerStats());
        }
        
        /// <summary>
        /// Perfect stats have max levels reached
        /// </summary>
        public static void SetPerfectPlayerStats()
        {
            Save(CreatePerfectPlayerStats());
        }
        
        public static void ResetSavedPlayerStats()
        {
            PersistentDataHelper.DeleteKey(PersistentDataKeys.PlayerStats);
        }
    }
}