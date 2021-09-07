using UnityEngine;

namespace Code.Level.Player
{
    [DefaultExecutionOrder(-1)]
    public class PlayerStatsManager : MonoBehaviour
    {
        private PlayerStats _playerStats;

        public PlayerStats PlayerStats => _playerStats;

        private void Awake()
        {
            _playerStats = PlayerStats.Load();
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
                _playerStats.UpdateFastestRecording(levelRecording, playerTookNoHits, out isFirstPerfect);
            }

            PlayerStats.Save(_playerStats);
        }

        public string SaveStats()
        {
            return PlayerStats.Save(_playerStats);
        }

        public void ResetTutorials()
        {
            _playerStats.UpdateCompletedTutorials(false, true);
            SaveStats();

        }
    }
}