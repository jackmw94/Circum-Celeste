using System.Collections;
using Code.Core;
using Code.Debugging;
using Code.Flow;
using Code.Level.Player;
using UnityEngine;

namespace Code.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private InterLevelFlow _interLevelFlow;
        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private PlayerContainer _playerContainer;
        [Space(15)]
        [SerializeField] private LevelGenerator _levelGenerator;
        [SerializeField] private PlayerStatsManager _playerStatsManager;
        
        private Coroutine _startLevelOnceMovedCoroutine = null;
        private bool _isReplaying = false;
        
        public LevelInstance CurrentLevel { get; private set; }
        
        public void RestartCurrentLevel()
        {
            if (!_levelProvider.GetCurrentLevel().LevelContext.IsFirstLevel)
            {
                _playerStatsManager.SetPlayerDied();
            }

            CreateCurrentLevel();
        }

        public void ClearCurrentLevel()
        {
            _levelGenerator.DestroyLevel();
        }
        
        public void CreateCurrentLevel(LevelRecording replay = null)
        {
            _isReplaying = replay != null;

            if (!_interLevelFlow.IsOverlaid)
            {
                _interLevelFlow.ShowHideUI(() =>
                {
                    CreateLevelInternal(replay);
                });
            }
            else
            {
                CreateLevelInternal(replay);
            }
        }

        private void CreateLevelInternal(LevelRecording replay)
        {
            LevelLayout levelLayout = _levelProvider.GetCurrentLevel();
            InputProvider[] inputProviders = _playerContainer.GetPlayerInputProviders(replay);
            LevelInstance levelInstance = GenerateLevel(levelLayout, inputProviders);

            if (levelLayout.LevelContext.IsFirstLevel)
            {
                _playerStatsManager.ResetCurrentRun();
            }
            
            _playerStatsManager.SetLevelIndex(levelLayout.LevelContext.LevelIndex, true);

            if (_startLevelOnceMovedCoroutine != null)
            {
                StopCoroutine(_startLevelOnceMovedCoroutine);
            }
            _startLevelOnceMovedCoroutine = StartCoroutine(StartLevelWhenReady(levelInstance));
        }

        private LevelInstance GenerateLevel(LevelLayout levelLayout, InputProvider[] inputProviders)
        {
            LevelInstance levelInstance = _levelGenerator.GenerateLevel(inputProviders, levelLayout);
            return levelInstance;
        }

        private IEnumerator StartLevelWhenReady(LevelInstance level)
        {
            _interLevelFlow.HideInterLevelUI();
            yield return new WaitUntil(() => !_interLevelFlow.IsOverlaid);
            
            level.LevelReady();
            
            yield return new WaitUntil(() => level.PlayerIsMoving);

            CircumDebug.Log($"Level '{level.name}' has started");
            CurrentLevel = level;
            level.StartLevel(OnLevelFinished);
        }

        private void OnLevelFinished(LevelResult levelResult)
        {
            if (levelResult.Success && !_isReplaying)
            {
                LevelLayout currentLevel = _levelProvider.GetCurrentLevel();
                LevelLayoutContext levelContext = currentLevel.LevelContext;
                
                LevelRecording levelRecording = new LevelRecording
                {
                    LevelIndex = levelContext.LevelIndex,
                    RecordingData = levelResult.LevelRecordingData
                };
                _playerStatsManager.UpdateStatisticsAfterLevel(currentLevel, levelResult.NoDamage, levelRecording);
                
                _levelProvider.AdvanceLevel();
            }
            
            _interLevelFlow.ShowInterLevelUI(ClearCurrentLevel);
        }
        
        [ContextMenu(nameof(ReplayLevel))]
        public void ReplayLevel()
        {
            LevelLayout currentLevel = _levelProvider.GetCurrentLevel();
            int levelIndex = currentLevel.LevelContext.LevelIndex;
            LevelRecording recording = _playerStatsManager.PlayerStats.GetRecordingForLevelAtIndex(levelIndex, false);
            
            CircumDebug.Assert(recording != null, $"Trying to replay level but could not find a replay for index {levelIndex}");
            CreateCurrentLevel(recording);
        }

        [ContextMenu(nameof(GenerateDebugLevel))]
        private LevelInstance GenerateDebugLevel()
        {
            InputProvider[] inputProviders = _playerContainer.GetPlayerInputProviders(null);
            return GenerateLevel(_levelProvider.GetCurrentLevel(), inputProviders);
        }
    }
}