using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Level.Player
{
    [DefaultExecutionOrder(-1)]
    public class PersistentDataManager : SingletonMonoBehaviour<PersistentDataManager>
    {
        private const string NoLoadingSavingPlayerPrefsKey = "Circum_DoNotLoadData";

        public class UpdatedStatisticsData
        {
            public BadgeData NewBadgeData;
            public NewFastestTimeInfo NewFastestTimeInfo;
            public bool FirstTimeCompletingLevel; 
            public bool ReplacedPreviousLevelRecord;
            public int AddedToScore;
        }

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
        private ChallengeData _challengeData;
        private Dictionary<string, LevelStats> _levelStats = new Dictionary<string, LevelStats>();
        private Dictionary<string, int> _cachedLevelScores = new Dictionary<string, int>();
        private int _cachedTotalScore = 0;

        public bool DoNotLoadOrSave => _doNotLoadOrSave;
        public PlayerStats PlayerStats => _playerStats;
        public PlayerFirsts PlayerFirsts => _playerFirsts;
        public CircumOptions Options => _circumOptions;
        public ChallengeData ChallengeData => _challengeData;

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

        public int GetScoreFromLevelName(string levelName)
        {
            return _cachedLevelScores.TryGetValue(levelName, out int score) ? score : -1;
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
        
        public PlayerScoreHelper.PlayerScore UpdateUserScore()
        {
            Dictionary<string, LevelLayout> levelLayoutsByName = _levelProvider
                .ActiveLevelProgression
                .LevelLayout
                .ToDictionary(layout => layout.name, layout => layout);

            PlayerScoreHelper.PlayerScore playerScore = PlayerScoreHelper.GetPlayerScore(levelLayoutsByName, _levelStats, _challengeData);
            
            _cachedLevelScores = playerScore.LevelScores.ToDictionary(levelScore => levelScore.LevelName, levelScore => levelScore.Score);
            _cachedTotalScore = playerScore.TotalScore;
            
            return playerScore;
        }

        public void SetLevelIndex(int levelIndex, bool save = false)
        {
            _playerStats.RunTracker.LevelIndex = levelIndex;
            if (save) SaveStats();
        }

        public LevelStats GetStatsForLevel(string levelName)
        {
            return !_levelStats.TryGetValue(levelName, out LevelStats levelStats) ? null : levelStats;
        }

        public UpdatedStatisticsData UpdateStatisticsAfterLevel(LevelLayout currentLevel, LevelRecording levelRecording)
        {
            UpdatedStatisticsData updatedStatisticsData = new UpdatedStatisticsData {NewBadgeData = new BadgeData(), NewFastestTimeInfo = null};
            
            bool isPerfect = levelRecording.RecordingData.IsPerfect;

            RunTracker runTracker = _playerStats.RunTracker;
            runTracker.IsPerfect &= isPerfect && runTracker.Deaths == 0;

            int levelIndex = currentLevel.LevelContext.LevelIndex;
            _playerStats.UpdateHighestLevel(levelIndex, runTracker.Deaths == 0, runTracker.IsPerfect, runTracker.HasSkipped, out updatedStatisticsData.FirstTimeCompletingLevel);

            _playerStats.UpdateCompletedTutorials(currentLevel.LevelContext.IsFinalTutorial);

            if (!currentLevel.LevelContext.IsTutorial)
            {
                if (!_levelStats.TryGetValue(currentLevel.name, out LevelStats levelStats))
                {
                    levelStats = new LevelStats();
                    _levelStats.Add(currentLevel.name, levelStats);
                }

                levelStats.UpdateFastestRecording(levelRecording, currentLevel.GoldTime, out updatedStatisticsData.NewBadgeData, out updatedStatisticsData.ReplacedPreviousLevelRecord);

                if (updatedStatisticsData.ReplacedPreviousLevelRecord)
                {
                    updatedStatisticsData.NewFastestTimeInfo = new NewFastestTimeInfo
                    {
                        Time = levelRecording.LevelTime,
                        IsPerfect = levelRecording.IsPerfect
                    };
                }
            }
            else
            {
                updatedStatisticsData.ReplacedPreviousLevelRecord = false;
            }

            updatedStatisticsData.AddedToScore = 0;
            if (updatedStatisticsData.ReplacedPreviousLevelRecord)
            {
                int previousTotalScore = _cachedTotalScore;
                PlayerScoreHelper.PlayerScore updatedScore = UpdateUserScore();
                if (previousTotalScore != updatedScore.TotalScore)
                {
                    updatedStatisticsData.AddedToScore = updatedScore.TotalScore - previousTotalScore;
                    RemoteDataManager.Instance.UpdateUserScore(updatedScore);
                }
            }

            SaveStats();

            return updatedStatisticsData;
        }

        private void LoadPersistentData()
        {
            if (_doNotLoadOrSave)
            {
                _playerStats = PlayerStats.CreateEmptyPlayerStats();
                _circumOptions = CircumOptions.CreateCircumOptions();
                _playerFirsts = new PlayerFirsts();
                _challengeData = new ChallengeData();
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

            _challengeData = ChallengeData.Load();
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
            ChallengeData.Save(_challengeData);
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

            _challengeData = new ChallengeData();
            ChallengeData.Save(_challengeData);
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

        public bool HasCompletedGame(bool requirePerfect)
        {
            foreach (LevelLayout levelLayout in _levelProvider.ActiveLevelProgression.LevelLayout)
            {
                if (!levelLayout.ContributesToScoring)
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

                bool levelNotComplete = !requirePerfect || hasPerfectRecording;
                
                if (levelNotComplete)
                {
                    return false;
                }
            }

            return true;
        }
    }
}