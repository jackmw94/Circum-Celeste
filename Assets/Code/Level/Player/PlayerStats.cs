﻿using System;
using System.Collections.Generic;
using System.Linq;
using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level.Player
{
    [Serializable]
    public class PlayerStats
    {
        private const string PlayerPrefsKey = "Circum_PlayerStats";
        private const int StatsDataVersion = 1;

        [SerializeField] private int _statsVersion;
        [SerializeField] private bool _completedTutorials;
        [SerializeField] private RunTracker _runTracker = null;
        [SerializeField] private List<LevelRecording> _levelRecordings;
        [SerializeField] private List<LevelRecording> _perfectLevelRecordings;
        
        [SerializeField] private int _highestLevelReachedIndex = 0;
        [SerializeField] private int _highestNoDeathLevelReachedIndex = 0;
        [SerializeField] private int _highestPerfectLevelReachedIndex = 0;

        public RunTracker RunTracker => _runTracker;
        public bool CompletedTutorials => _completedTutorials;
        public int HighestLevelIndex => _highestLevelReachedIndex;
        public int HighestLevelNoDeathsIndex => _highestNoDeathLevelReachedIndex;
        public int HighestPerfectLevelIndex => _highestPerfectLevelReachedIndex;

        public void UpdateFastestRecording(LevelRecording levelRecording, bool perfect)
        {
            UpdateFastestRecordingInternal(levelRecording, _levelRecordings);
            if (perfect)
            {
                UpdateFastestRecordingInternal(levelRecording, _perfectLevelRecordings);
            }
        }

        private void UpdateFastestRecordingInternal(LevelRecording levelRecording, List<LevelRecording> recordingsList)
        {
            LevelRecording currentRecordingForLevel = recordingsList.FirstOrDefault(p => p.LevelIndex == levelRecording.LevelIndex);
            
            if (currentRecordingForLevel == null)
            {
                recordingsList.Add(levelRecording);
            }
            else if (currentRecordingForLevel.RecordingData.LevelTime > levelRecording.RecordingData.LevelTime)
            {
                recordingsList.Remove(currentRecordingForLevel);
                recordingsList.Add(levelRecording);
            }
        }

        public LevelRecording GetRecordingForLevelAtIndex(int levelIndex, bool perfect)
        {
            List<LevelRecording> recordings = perfect ? _perfectLevelRecordings : _levelRecordings;
            return recordings.FirstOrDefault(p => p.LevelIndex == levelIndex);
        }
        
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
            return $"Tutorials complete = {_completedTutorials}\nHighest level reached = {_highestLevelReachedIndex}\nHighest level with no deaths = {_highestNoDeathLevelReachedIndex}\nHighest level on perfect run = {_highestPerfectLevelReachedIndex}\nLast run = {_runTracker}\n{(_levelRecordings == null ? "NULL" : _levelRecordings.JoinToString("\n"))}";
        }

        public static void Save(PlayerStats stats)
        {
            stats._statsVersion = StatsDataVersion;
            
            string serialized = JsonUtility.ToJson(stats);
            PlayerPrefs.SetString(PlayerPrefsKey, serialized);
            PlayerPrefs.Save();
            CircumDebug.Log($"Saved player stats: {stats}");
        }
        
        public static PlayerStats Load()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                CircumDebug.Log("Created new player stats since we can't find saved key");
                return CreateEmptyPlayerStats();
            }
            
            string serializedPlayerStats = PlayerPrefs.GetString(PlayerPrefsKey);
            PlayerStats deserializedPlayerStats = JsonUtility.FromJson<PlayerStats>(serializedPlayerStats);
            CircumDebug.Log($"Loaded player stats: {deserializedPlayerStats}");

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
                _runTracker = new RunTracker(),
                _levelRecordings = new List<LevelRecording>(),
                _perfectLevelRecordings = new List<LevelRecording>()
            };
        }
        
        public static PlayerStats CreateStarterPlayerStats()
        {
            return new PlayerStats
            {
                _completedTutorials = true,
                _runTracker = new RunTracker(),
                _levelRecordings = new List<LevelRecording>(),
                _perfectLevelRecordings = new List<LevelRecording>()
            };
        }

        /// <summary>
        /// Starter stats are empty but have tutorials completed flag set to true
        /// </summary>
        public static void SetStarterPlayerStats()
        {
            Save(CreateStarterPlayerStats());
        }
        
        public static void ResetSavedPlayerStats()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKey);
        }
    }
}