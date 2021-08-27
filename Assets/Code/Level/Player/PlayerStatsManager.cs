using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerStatsManager : MonoBehaviour
    {
        private PlayerStats _playerStats;

        public PlayerStats PlayerStats => _playerStats;

        private void Awake()
        {
            _playerStats = PlayerStats.Load();
        }
        
        public int GetRestartLevel()
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
        
        public void UpdateStatisticsAfterLevel(LevelLayout currentLevel, bool playerTookNoHits, LevelRecording levelRecording)
        {
            RunTracker runTracker = _playerStats.RunTracker;
            runTracker.IsPerfect &= playerTookNoHits && runTracker.Deaths == 0;
            
            int userFacingLevelIndex = currentLevel.LevelContext.LevelNumber;
            _playerStats.UpdateHighestLevel(userFacingLevelIndex, runTracker.Deaths == 0, runTracker.IsPerfect, runTracker.HasSkipped);

            // can still set high scores
            _playerStats.UpdateCompletedTutorials(currentLevel.LevelContext.IsTutorial);
            _playerStats.UpdateFastestRecording(levelRecording);
            
            PlayerStats.Save(_playerStats);
        }

        private void SaveStats()
        {
            PlayerStats.Save(_playerStats);
        }
    }
}