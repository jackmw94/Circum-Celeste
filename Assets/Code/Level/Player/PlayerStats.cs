﻿using System;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class PlayerStats
    {
        private const string PlayerPrefsKey = "Circum_PlayerStatistics";
        private const int StatsDataVersion = 2;
        
        [SerializeField] private int _statsVersion;
        [SerializeField] private bool _completedTutorials;
        [SerializeField] private RunTracker _runTracker = null;
        
        [SerializeField] private int _highestLevelReachedIndex = 0;
        [SerializeField] private int _highestNoDeathLevelReachedIndex = 0;
        [SerializeField] private int _highestPerfectLevelReachedIndex = 0;

        public RunTracker RunTracker => _runTracker;
        public bool CompletedTutorials => _completedTutorials;
        public int HighestLevelIndex => _highestLevelReachedIndex;
        public int HighestLevelNoDeathsIndex => _highestNoDeathLevelReachedIndex;
        public int HighestPerfectLevelIndex => _highestPerfectLevelReachedIndex;

        
        public void UpdateHighestLevel(int levelIndex, bool noDeaths, bool noHits, bool hasSkipped)
        {
            _highestLevelReachedIndex = Mathf.Max(levelIndex + 1, _highestLevelReachedIndex);

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
            stats._statsVersion = StatsDataVersion;
            
            string serialized = JsonUtility.ToJson(stats);
            string compressed = serialized.Compress();

            CircumPlayerPrefs.SetString(PlayerPrefsKey, compressed);
            CircumPlayerPrefs.Save();
            CircumDebug.Log($"Saved player stats: {stats}");
        }
        
        public static PlayerStats Load()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                CircumDebug.Log("Created new player stats since we can't find saved key");
                return CreateEmptyPlayerStats();
            }
            
            string serializedPlayerStats = CircumPlayerPrefs.GetString(PlayerPrefsKey);

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

            if (deserializedPlayerStats._statsVersion != StatsDataVersion)
            {
                CircumDebug.Log($"Player stats were of a previous version (curr={StatsDataVersion}, loaded={deserializedPlayerStats._statsVersion}), deleted and returned empty.");
                ResetSavedPlayerStats();
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
            Save(CreateStarterPlayerStats());
        }
        
        public static void ResetSavedPlayerStats()
        {
            CircumPlayerPrefs.DeleteKey(PlayerPrefsKey);
        }
    }
}