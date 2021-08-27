using System;
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

        [SerializeField] private bool _completedTutorials;
        [SerializeField] private RunTracker _runTracker = null;
        [SerializeField] private List<LevelRecording> _levelRecordings;
        
        // these level ints are user facing numbers, not indexes. should probably clear up that distinction
        [SerializeField] private int _highestLevelReached = 0;
        [SerializeField] private int _highestNoDeathLevelReached = 0;
        [SerializeField] private int _highestPerfectLevelReached = 0;
        //

        public RunTracker RunTracker => _runTracker;
        public bool CompletedTutorials => _completedTutorials;
        public int HighestLevel => _highestLevelReached;
        public int HighestLevelNoDeaths => _highestNoDeathLevelReached;
        public int HighestPerfectLevel => _highestPerfectLevelReached;

        public void UpdateFastestRecording(LevelRecording levelRecording)
        {
            LevelRecording currentRecordingForLevel = _levelRecordings.FirstOrDefault(p => p.LevelIndex == levelRecording.LevelIndex);
            if (currentRecordingForLevel == null)
            {
                _levelRecordings.Add(levelRecording);
            }
            else if (currentRecordingForLevel.RecordingData.LevelTime > levelRecording.RecordingData.LevelTime)
            {
                _levelRecordings.Remove(currentRecordingForLevel);
                _levelRecordings.Add(levelRecording);
            }
        }

        public LevelRecording GetRecordingForLevel(int levelIndex)
        {
            return _levelRecordings.FirstOrDefault(p => p.LevelIndex == levelIndex);
        }
        
        public void UpdateHighestLevel(int levelNumber, bool noDeaths, bool noHits, bool hasSkipped)
        {
            _highestLevelReached = Mathf.Max(levelNumber, _highestLevelReached);

            if (hasSkipped)
            {
                return;
            }

            if (noDeaths)
            {
                _highestNoDeathLevelReached = Mathf.Max(levelNumber, _highestNoDeathLevelReached);
            }
            
            if (noHits)
            {
                _highestPerfectLevelReached = Mathf.Max(levelNumber, _highestPerfectLevelReached);
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
            return $"Tutorials complete = {_completedTutorials}\nHighest level reached = {_highestLevelReached}\nHighest level with no deaths = {_highestNoDeathLevelReached}\nHighest level on perfect run = {_highestPerfectLevelReached}\nLast run = {_runTracker}\n{(_levelRecordings == null ? "NULL" : _levelRecordings.JoinToString("\n"))}";
        }

        public static void Save(PlayerStats stats)
        {
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
                return new PlayerStats()
                {
                    _runTracker = new RunTracker(),
                    _levelRecordings = new List<LevelRecording>()
                };
            }
            
            string serializedPlayerStats = PlayerPrefs.GetString(PlayerPrefsKey);
            PlayerStats deserializedPlayerStats = JsonUtility.FromJson<PlayerStats>(serializedPlayerStats);
            CircumDebug.Log($"Loaded player stats: {deserializedPlayerStats}");

            if (deserializedPlayerStats._runTracker == null)
            {
                deserializedPlayerStats._runTracker = new RunTracker();
            }
            return deserializedPlayerStats;
        }
        
        public static void ResetSavedPlayerStats()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKey);
        }
    }
}