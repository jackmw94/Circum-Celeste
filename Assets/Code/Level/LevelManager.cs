using System;
using System.Collections;
using System.Linq;
using Code.Core;
using Code.Debugging;
using Code.Flow;
using Code.Level.Player;
using UnityEngine;

namespace Code.Level
{
    public class LevelManager : MonoBehaviour, IValidateable
    {
        private enum InputType
        {
            Keyboard,
            Mouse,
            Controller,
            UI
        }

        [SerializeField] private LevelProvider _levelProvider;
        [Space(15)] 
        [SerializeField] private InputType[] _playersInputs;
        [Space(15)] 
        [SerializeField] private LevelGenerator _levelGenerator;
        [SerializeField] private PlayerStatsManager _playerStatsManager;
        
        private Coroutine _startLevelOnceMovedCoroutine = null;
        
        public LevelInstance CurrentLevel { get; private set; }

        private void Awake()
        {
            CircumDebug.Assert(_playersInputs.Length > 0, "There are no users for this level, probably not what we want..");
            
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
                
                CreateCurrentLevel();
            });
        }
        
        public LevelLayout GetNextLevel()
        {
            return _levelProvider.GetNextLevel();
        }

        public LevelLayout GetCurrentLevel()
        {
            return _levelProvider.GetCurrentLevel();
        }

        public void SkipLevel()
        {
            _playerStatsManager.SetSkippedLevel();
            AdvanceLevel();
        }

        private void AdvanceLevel()
        {
            _levelProvider.AdvanceLevel();
            CreateCurrentLevel();
        }

        public void ResetCurrentLevel()
        {
            if (!_levelProvider.GetCurrentLevel().IsFirstLevel)
            {
                _playerStatsManager.SetPlayerDied();
            }

            CreateCurrentLevel();
        }

        public void ResetRun()
        {
            _levelProvider.ResetToStart();
            CreateCurrentLevel();
        }

        private void CreateCurrentLevel()
        {
            LevelLayout levelLayout = _levelProvider.GetCurrentLevel();
            LevelInstance levelInstance = GenerateLevel(levelLayout);

            if (levelLayout.IsFirstLevel)
            {
                _playerStatsManager.ResetCurrentRun();
            }
            
            _playerStatsManager.SetLevelIndex(levelLayout.LevelNumber - 1, true);

            if (_startLevelOnceMovedCoroutine != null)
            {
                StopCoroutine(_startLevelOnceMovedCoroutine);
            }
            _startLevelOnceMovedCoroutine = StartCoroutine(StartLevelOncePlayerMoved(levelInstance));
        }

        [ContextMenu(nameof(GenerateDebugLevel))]
        private LevelInstance GenerateDebugLevel()
        {
            return GenerateLevel(_levelProvider.GetCurrentLevel());
        }

        private LevelInstance GenerateLevel(LevelLayout levelLayout)
        {
            InputProvider[] userInputProviders = _playersInputs.Select(CreateInputProvider).ToArray();
            LevelInstance levelInstance = _levelGenerator.GenerateLevel(userInputProviders, levelLayout);

            return levelInstance;
        }

        private IEnumerator StartLevelOncePlayerMoved(LevelInstance level)
        {
            yield return new WaitUntil(() => !FlowContainer.Instance.InterLevelFlow.IsOverlaid);
            level.LevelReady();
            yield return new WaitUntil(() => level.PlayerIsMoving);

            CircumDebug.Log("Level started");
            CurrentLevel = level;
            level.StartLevel(OnLevelFinished);
        }

        private void OnLevelFinished(bool success, bool playerTookNoHits)
        {
            InterLevelFlow interLevelFlow = FlowContainer.Instance.InterLevelFlow;
            interLevelFlow.ShowOverlayInstant = false;
            interLevelFlow.ShowNextLevelName = success;
            
            if (success)
            {
                _playerStatsManager.UpdateStatisticsAfterLevel(playerTookNoHits, _levelProvider.GetCurrentLevel());
                interLevelFlow.StartAction(AdvanceLevel);
            }
            else
            {
                interLevelFlow.StartAction(ResetCurrentLevel);
            }
        }
        
        private static InputProvider CreateInputProvider(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Controller:
                    return InputProvider.CreateInputProvider<ControllerInputProvider>();
                case InputType.Keyboard:
                    return InputProvider.CreateInputProvider<KeyboardInputProvider>();
                case InputType.Mouse:
                    return InputProvider.CreateInputProvider<MouseInputProvider>();
                case InputType.UI:
                    return InputProvider.CreateInputProvider<UIInputProvider>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputType), inputType, "Cannot create input provider");
            }
        }

        public void Validate()
        {
             Debug.Assert(_playersInputs.Length == 1 && _playersInputs[0] == InputType.UI, 
                            $"We're not building with expected player inputs! There are {_playersInputs.Length}{(_playersInputs.Length > 0 ? $" and the first is {_playersInputs[0]}" : "")}");
        }
    }
}