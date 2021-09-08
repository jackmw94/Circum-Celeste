using System.Collections.Generic;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Level.Player
{
    [DefaultExecutionOrder(-1)]
    public class PersistentDataManager : SingletonMonoBehaviour<PersistentDataManager>
    {
        private const string NoLoadingSavingPlayerPrefsKey = "Circum_DoNotLoadData";
        
        [SerializeField] private LevelProvider _levelProvider;

        private bool _doNotLoadOrSave;
        private CircumOptions _circumOptions;
        private PlayerStats _playerStats;
        private Dictionary<string, LevelStats> _levelStats = new Dictionary<string, LevelStats>();

        public bool DoNotLoadOrSave => _doNotLoadOrSave;
        public PlayerStats PlayerStats => _playerStats;
        public CircumOptions Options => _circumOptions;

        private void Awake()
        {
            _doNotLoadOrSave = GetDoNotLoadOrSave();
            LoadPersistentData();
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

        public void SetShowHideLevelTimer(bool showLevelTimer)
        {
            _circumOptions.ShowLevelTimer = showLevelTimer;
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
            if (!_levelStats.TryGetValue(levelName, out LevelStats levelStats))
            {
                return null;
            }
            
            // found level stats
            
            if (perfect)
            {
                return levelStats.HasFastestPerfectLevelRecording ? levelStats.FastestPerfectLevelRecording : null;
            }

            return levelStats.HasFastestLevelRecording ? levelStats.FastestLevelRecording : null;
        }
        
        public void UpdateStatisticsAfterLevel(LevelLayout currentLevel, bool playerTookNoHits, LevelRecording levelRecording, out BadgeData newBadgeData)
        {
            newBadgeData = new BadgeData();
            
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
                    levelStats.UpdateFastestRecording(levelRecording, playerTookNoHits, currentLevel.GoldTime, out newBadgeData);
                }
                else
                {
                    LevelStats newStats = new LevelStats();
                    _levelStats.Add(currentLevel.name, newStats);
                    newStats.UpdateFastestRecording(levelRecording, playerTookNoHits, currentLevel.GoldTime, out newBadgeData);
                }
            }

            SaveStats();
        }

        private void LoadPersistentData()
        {
            if (_doNotLoadOrSave)
            {
                _playerStats = PlayerStats.CreateEmptyPlayerStats();
                return;
            }
            
            _playerStats = PlayerStats.Load();
            foreach (LevelLayout levelLayout in _levelProvider.ActiveLevelProgression.LevelLayout)
            {
                if (LevelStats.TryLoadLevelStats(levelLayout.name, out LevelStats levelStats))
                {
                    _levelStats.Add(levelLayout.name, levelStats);
                }
            }

            _circumOptions = CircumOptions.Load();
        }
        
        private void SaveStats()
        {
            if (_doNotLoadOrSave)
            {
                return;
            }
            
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

        public void ResetStats()
        {
            PlayerStats.ResetSavedPlayerStats();
            foreach (LevelLayout levels in _levelProvider.ActiveLevelProgression.LevelLayout)
            {
                LevelStats.ResetStats(levels.name);
            }
            CircumOptions.ResetOptions();
        }

        public void SetDoNotLoadOrSave(bool doNotLoadOrSave)
        {
            _doNotLoadOrSave = doNotLoadOrSave;
            int toggledValue = _doNotLoadOrSave ? 1 : 0;
            CircumPlayerPrefs.SetInt(NoLoadingSavingPlayerPrefsKey, toggledValue);
        }

        private bool GetDoNotLoadOrSave()
        {
            return CircumPlayerPrefs.HasKey(NoLoadingSavingPlayerPrefsKey) && CircumPlayerPrefs.GetInt(NoLoadingSavingPlayerPrefsKey) == 1;
        }
    }
}