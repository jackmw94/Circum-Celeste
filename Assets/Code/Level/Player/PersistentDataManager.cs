using System.Collections;
using System.Collections.Generic;
using Code.Core;
using Code.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Level.Player
{
    [DefaultExecutionOrder(-1)]
    public class PersistentDataManager : SingletonMonoBehaviour<PersistentDataManager>
    {
        private const string NoLoadingSavingPlayerPrefsKey = "Circum_DoNotLoadData";

#if UNITY_EDITOR
        public bool ForceOldSaveMethod;
#endif

        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private float _levelIndexUpdateRate = 4f;

        private Coroutine _updateLevelIndexCoroutine = null;
        private bool _doNotLoadOrSave;
        private CircumOptions _circumOptions;
        private PlayerFirsts _playerFirsts;
        private PlayerStats _playerStats;
        private Dictionary<string, LevelStats> _levelStats = new Dictionary<string, LevelStats>();

        public bool DoNotLoadOrSave => _doNotLoadOrSave;
        public PlayerStats PlayerStats => _playerStats;
        public PlayerFirsts PlayerFirsts => _playerFirsts;
        public CircumOptions Options => _circumOptions;

        private void Awake()
        {
            _doNotLoadOrSave = GetDoNotLoadOrSave();
            LoadPersistentData();

#if UNITY_EDITOR
            if (SceneManager.sceneCount == 1)
            {
                // we started editor in game scene
                RemoteConfigHelper.RequestRefresh();
            }
#endif
        }
        
        public int GetRestartLevelIndex()
        {
            return _playerStats.RestartLevelIndex;
        }

        public void SetCurrentLevel(int currentLevelIndex)
        {
            _playerStats.SetCurrentLevel(currentLevelIndex);

            // delay to prevent exceeding remote data update rate
            if (_updateLevelIndexCoroutine != null)
            {
                StopCoroutine(_updateLevelIndexCoroutine);
            }

            _updateLevelIndexCoroutine = StartCoroutine(UpdateCurrentLevelCoroutine());
        }

        private IEnumerator UpdateCurrentLevelCoroutine()
        {
            yield return new WaitForSeconds(_levelIndexUpdateRate);
            PlayerStats.Save(_playerStats);
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

        public LevelStats GetStatsForLevelAtIndex(string levelName)
        {
            return !_levelStats.TryGetValue(levelName, out LevelStats levelStats) ? null : levelStats;
        }

        public void UpdateStatisticsAfterLevel(LevelLayout currentLevel, LevelRecording levelRecording, out BadgeData newBadgeData, out NewFastestTimeInfo newFastestTimeInfo,
            out bool firstTimeCompletingLevel, out bool replacedPreviousLevelRecord)
        {
            newBadgeData = new BadgeData();
            newFastestTimeInfo = null;

            bool isPerfect = levelRecording.RecordingData.IsPerfect;

            RunTracker runTracker = _playerStats.RunTracker;
            runTracker.IsPerfect &= isPerfect && runTracker.Deaths == 0;

            int levelIndex = currentLevel.LevelContext.LevelIndex;
            _playerStats.UpdateHighestLevel(levelIndex, runTracker.Deaths == 0, runTracker.IsPerfect, runTracker.HasSkipped, out firstTimeCompletingLevel);

            _playerStats.UpdateCompletedTutorials(currentLevel.LevelContext.IsFinalTutorial);

            if (!currentLevel.LevelContext.IsTutorial)
            {
                if (!_levelStats.TryGetValue(currentLevel.name, out LevelStats levelStats))
                {
                    levelStats = new LevelStats();
                    _levelStats.Add(currentLevel.name, levelStats);
                }

                levelStats.UpdateFastestRecording(levelRecording, currentLevel.GoldTime, out newBadgeData, out replacedPreviousLevelRecord);

                if (replacedPreviousLevelRecord)
                {
                    newFastestTimeInfo = new NewFastestTimeInfo
                    {
                        Time = levelRecording.LevelTime,
                        IsPerfect = levelRecording.IsPerfect
                    };
                }
            }
            else
            {
                replacedPreviousLevelRecord = false;
            }

            SaveStats();
        }

        private void LoadPersistentData()
        {
            if (_doNotLoadOrSave)
            {
                _playerStats = PlayerStats.CreateEmptyPlayerStats();
                _circumOptions = CircumOptions.CreateCircumOptions();
                _playerFirsts = new PlayerFirsts();
                return;
            }

            _playerStats = PlayerStats.Load();
            foreach (LevelLayout levelLayout in _levelProvider.ActiveLevelProgression.LevelLayout)
            {
                if (LevelStats.TryLoadLevelStats(levelLayout.name, levelLayout.GoldTime, out LevelStats levelStats))
                {
                    _levelStats.Add(levelLayout.name, levelStats);
                }
            }

            _playerFirsts = PlayerFirsts.Load();
            _circumOptions = CircumOptions.Load();
        }

        private void SaveStats()
        {
            if (_doNotLoadOrSave)
            {
                return;
            }

            PlayerStats.Save(_playerStats);
            foreach (KeyValuePair<string, LevelStats> statsForLevel in _levelStats)
            {
                string levelName = statsForLevel.Key;
                LevelStats stats = statsForLevel.Value;

                LevelStats.SaveLevelStats(levelName, stats);
            }

            PlayerFirsts.Save(_playerFirsts);
            CircumOptions.Save(_circumOptions);
        }

        public void ResetTutorials()
        {
            _playerStats.UpdateCompletedTutorials(false, true);
            SaveStats();
        }

        public void ResetStats()
        {
            PlayerStats.ResetSavedPlayerStats();
            _playerStats = PlayerStats.CreateEmptyPlayerStats();

            foreach (LevelLayout level in _levelProvider.ActiveLevelProgression.LevelLayout)
            {
                LevelStats.ResetStats(level.name);
            }

            _levelStats.Clear();

            CircumOptions.ResetOptions();
            _circumOptions = CircumOptions.CreateCircumOptions();

            PlayerFirsts.ResetPlayerFirsts();
            _playerFirsts = new PlayerFirsts();

            PersistentDataHelper.DeleteKey(PersistentDataKeys.SplashScreenLastRunTime, true);
        }

        public void SetDoNotLoadOrSave(bool doNotLoadOrSave)
        {
            _doNotLoadOrSave = doNotLoadOrSave;
            int toggledValue = _doNotLoadOrSave ? 1 : 0;
            PersistentDataHelper.SetInt(NoLoadingSavingPlayerPrefsKey, toggledValue);
        }

        private bool GetDoNotLoadOrSave()
        {
            return PersistentDataHelper.HasKey(NoLoadingSavingPlayerPrefsKey) && PersistentDataHelper.GetInt(NoLoadingSavingPlayerPrefsKey) == 1;
        }

        public bool HasCompletedGame()
        {
            foreach (LevelLayout levelLayout in _levelProvider.ActiveLevelProgression.LevelLayout)
            {
                if (!levelLayout.RequiredForGameCompletion)
                {
                    continue;
                }

                if (!_levelStats.TryGetValue(levelLayout.name, out LevelStats levelStats))
                {
                    // no level stats for required level
                    return false;
                }

                bool hasPerfectRecording = levelStats.HasRecording &&
                                           levelStats.LevelRecording.RecordingData.IsPerfect &&
                                           levelStats.LevelRecording.HasBeatenGoldTime(levelLayout.GoldTime);

                if (!hasPerfectRecording)
                {
                    return false;
                }
            }

            return true;
        }
    }
}