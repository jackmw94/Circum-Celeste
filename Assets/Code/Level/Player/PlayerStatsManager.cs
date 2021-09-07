﻿using System.Collections.Generic;
using UnityEngine;

namespace Code.Level.Player
{
    [DefaultExecutionOrder(-1)]
    public class PlayerStatsManager : MonoBehaviour
    {
        [SerializeField] private LevelProvider _levelProvider;
        
        private PlayerStats _playerStats;
        private Dictionary<string, LevelStats> _levelStats = new Dictionary<string, LevelStats>();

        public PlayerStats PlayerStats => _playerStats;

        private void Awake()
        {
            _playerStats = PlayerStats.Load();
            foreach (LevelLayout levelLayout in _levelProvider.ActiveLevelProgression.LevelLayout)
            {
                if (LevelStats.TryLoadLevelStats(levelLayout.name, out LevelStats levelStats))
                {
                    _levelStats.Add(levelLayout.name, levelStats);
                }
            }
        }
        
        public int GetRestartLevelIndex()
        {
            RunTracker lastRun = _playerStats.RunTracker;
            if (lastRun == null)
            {
                return 0;
            }
            
            return lastRun.HasSkipped ? 0 : lastRun.LevelIndex;
        }

        public void SetSkippedLevel(bool save = false)
        {
            _playerStats.RunTracker.HasSkipped = true;
            if (save) SaveStats();
        }

        public void ResetCurrentRun(bool save = false)
        {
            _playerStats.RunTracker.ResetRun();
            if (save) SaveStats();
        }

        public void SetPlayerDied(bool save = false)
        {
            _playerStats.RunTracker.Deaths++;
            _playerStats.RunTracker.IsPerfect = false;
            if (save) SaveStats();
        }

        public void SetLevelIndex(int levelIndex, bool save = false)
        {
            _playerStats.RunTracker.LevelIndex = levelIndex;
            if (save) SaveStats();
        }

        public LevelRecording GetRecordingForLevelAtIndex(string levelName, bool perfect)
        {
            if (_levelStats.TryGetValue(levelName, out LevelStats levelStats))
            {
                return perfect ? levelStats.FastestPerfectLevelRecording : levelStats.FastestLevelRecording;
            }

            return null;
        }
        
        public void UpdateStatisticsAfterLevel(LevelLayout currentLevel, bool playerTookNoHits, LevelRecording levelRecording, out bool isFirstPerfect)
        {
            isFirstPerfect = false;
            
            RunTracker runTracker = _playerStats.RunTracker;
            runTracker.IsPerfect &= playerTookNoHits && runTracker.Deaths == 0;
            
            int levelIndex = currentLevel.LevelContext.LevelIndex;
            _playerStats.UpdateHighestLevel(levelIndex, runTracker.Deaths == 0, runTracker.IsPerfect, runTracker.HasSkipped);
            
            bool levelIsTutorial = currentLevel.LevelContext.IsTutorial;
            _playerStats.UpdateCompletedTutorials(levelIsTutorial);

            if (!levelIsTutorial)
            {
                if (_levelStats.TryGetValue(currentLevel.name, out LevelStats levelStats))
                {
                    levelStats.UpdateFastestRecording(levelRecording, playerTookNoHits, out isFirstPerfect);
                }
                else
                {
                    LevelStats newStats = new LevelStats();
                    _levelStats.Add(currentLevel.name, newStats);
                    newStats.UpdateFastestRecording(levelRecording, playerTookNoHits, out isFirstPerfect);
                }
            }

            SaveStats();
        }

        public void SaveStats()
        {
            PlayerStats.Save(_playerStats);
            foreach (KeyValuePair<string,LevelStats> statsForLevel in _levelStats)
            {
                string levelName = statsForLevel.Key;
                LevelStats stats = statsForLevel.Value;
                
                LevelStats.SaveLevelStats(levelName, stats);
            }
        }

        public void ResetTutorials()
        {
            _playerStats.UpdateCompletedTutorials(false, true);
            SaveStats();
        }
    }
}