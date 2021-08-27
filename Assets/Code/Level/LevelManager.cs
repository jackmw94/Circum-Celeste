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
        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private PlayerContainer _playerContainer;
        [Space(15)]
        [SerializeField] private LevelGenerator _levelGenerator;
        [SerializeField] private PlayerStatsManager _playerStatsManager;
        
        private Coroutine _startLevelOnceMovedCoroutine = null;
        private bool _isReplaying = false;
        
        public LevelInstance CurrentLevel { get; private set; }

        private void Awake()
        {
            int restartLevel = _playerStatsManager.GetRestartLevel();
            _levelProvider.Initialise(_playerStatsManager.PlayerStats.CompletedTutorials, restartLevel);
            
            InterLevelFlow interLevelFlow = FlowContainer.Instance.InterLevelFlow;
            interLevelFlow.ShowOverlayInstant = true;
            interLevelFlow.ShowNextLevelName = false;
            interLevelFlow.PreventHidingOverlay = true;
            interLevelFlow.StartAction(null);

            RemoteConfigHelper.RequestRefresh(success =>
            {
                CircumDebug.Log("Remote config request finished, showing level");
                interLevelFlow.PreventHidingOverlay = false;
                
                CreateCurrentLevel(null);
            });
        }

        public void SkipLevel()
        {
            _playerStatsManager.SetSkippedLevel();
            AdvanceLevel();
        }

        private void AdvanceLevel()
        {
            _levelProvider.AdvanceLevel();
            CreateCurrentLevel(null);
        }

        public void ResetCurrentLevel()
        {
            if (!_levelProvider.GetCurrentLevel().LevelContext.IsFirstLevel)
            {
                _playerStatsManager.SetPlayerDied();
            }

            CreateCurrentLevel(null);
        }

        public void ResetRun()
        {
            _levelProvider.ResetToStart();
            CreateCurrentLevel(null);
        }

        private void CreateCurrentLevel(LevelRecording replay)
        {
            _isReplaying = replay != null;
            
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
            _startLevelOnceMovedCoroutine = StartCoroutine(StartLevelOncePlayerMoved(levelInstance));
        }

        private LevelInstance GenerateLevel(LevelLayout levelLayout, InputProvider[] inputProviders)
        {
            LevelInstance levelInstance = _levelGenerator.GenerateLevel(inputProviders, levelLayout);
            return levelInstance;
        }

        private IEnumerator StartLevelOncePlayerMoved(LevelInstance level)
        {
            yield return new WaitUntil(() => !FlowContainer.Instance.InterLevelFlow.IsOverlaid);
            level.LevelReady();
            yield return new WaitUntil(() => level.PlayerIsMoving);

            CircumDebug.Log($"Level '{level.name}' has started");
            CurrentLevel = level;
            level.StartLevel(OnLevelFinished);
        }

        private void  OnLevelFinished(LevelResult levelResult)
        {
            InterLevelFlow interLevelFlow = FlowContainer.Instance.InterLevelFlow;
            interLevelFlow.ShowOverlayInstant = false;
            interLevelFlow.ShowNextLevelName = levelResult.Success;
            
            if (levelResult.Success && !_isReplaying)
            {
                LevelRecording levelRecording = new LevelRecording
                {
                    LevelIndex = _levelProvider.GetCurrentLevel().LevelContext.LevelIndex,
                    RecordingData = levelResult.LevelRecordingData
                };
                _playerStatsManager.UpdateStatisticsAfterLevel(_levelProvider.GetCurrentLevel(), levelResult.NoDamage, levelRecording);
                interLevelFlow.StartAction(AdvanceLevel);
            }
            else
            {
                interLevelFlow.StartAction(ResetCurrentLevel);
            }
        }

        public LevelLayout GetNextLevel()
        {
            return _levelProvider.GetNextLevel();
        }

        public LevelLayout GetCurrentLevel()
        {
            return _levelProvider.GetCurrentLevel();
        }

        [ContextMenu(nameof(ReplayLevel))]
        public void ReplayLevel()
        {
            LevelLayout currentLevel = _levelProvider.GetCurrentLevel();
            int levelIndex = currentLevel.LevelContext.LevelIndex;
            LevelRecording recording = _playerStatsManager.PlayerStats.GetRecordingForLevel(levelIndex);
            
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