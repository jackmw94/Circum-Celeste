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
    public class LevelManager : MonoBehaviour
    {
        public enum InputType
        {
            Keyboard,
            Mouse,
            Controller,
            UI
        }

        [SerializeField] private LevelProgression _levelProgression;
        [Space(15)] 
        [SerializeField] private InputType[] _playersInputs;
        [Space(15)] 
        [SerializeField] private LevelGenerator _levelGenerator;

        private int _currentLevelIndex = 0;
        
        private RunTracker _runTracker;
        private PlayerStats _playerStats;

        private int NextLevelIndex => (_currentLevelIndex + 1) % _levelProgression.LevelLayout.Length;
        public InputType[] PlayersInputs => _playersInputs;
        public PlayerStats PlayerStats => _playerStats;
        
        public LevelInstance CurrentLevel { get; private set; }

        private void Awake()
        {
            CircumDebug.Assert(PlayersInputs.Length > 0, "There are no users for this level, probably not what we want..");
            
            _playerStats = PlayerStats.Load();
            
            InterLevelFlow interLevelFlow = FlowContainer.Instance.InterLevelFlow;
            interLevelFlow.ShowOverlayInstant = true;
            interLevelFlow.ShowNextLevelName = false;
            interLevelFlow.PreventHidingOverlay = true;
            interLevelFlow.StartAction(null);
            
            RemoteConfigHelper.RequestRefresh(success =>
            {
                CircumDebug.Log("Remote config request finished, showing level");
                interLevelFlow.PreventHidingOverlay = false;
                
                GenerateFirstLevel();
            });
        }

        public LevelLayout GetNextLevel(out int index)
        {
            index = NextLevelIndex;
            return _levelProgression.LevelLayout[index];
        }

        public LevelLayout GetCurrentLevel(out int index)
        {
            index = _currentLevelIndex;
            return _levelProgression.LevelLayout[index];
        }

        public void SkipLevel()
        {
            _runTracker.HasSkipped = true;
            GenerateNextLevel();
        }

        private void GenerateNextLevel()
        {
            _currentLevelIndex = NextLevelIndex;
            CreateLevel();
        }

        public void ResetCurrentLevel()
        {
            if (_currentLevelIndex > 0)
            {
                _runTracker.Deaths++;
            }

            CreateLevel();
        }

        public void GenerateFirstLevel()
        {
            _currentLevelIndex = 0;
            _runTracker = new RunTracker();
            CreateLevel();
        }

        private void CreateLevel()
        {
            LevelLayout levelLayout = _levelProgression.LevelLayout[_currentLevelIndex];
            
            LevelInstance levelInstance = GenerateLevel(levelLayout);
            StartCoroutine(StartLevelOncePlayerMoved(levelInstance));
        }

        [ContextMenu(nameof(GenerateDebugLevel))]
        private LevelInstance GenerateDebugLevel()
        {
            return GenerateLevel(_levelProgression.LevelLayout[0]);
        }

        private LevelInstance GenerateLevel(LevelLayout levelLayout)
        {
            InputProvider[] userInputProviders = PlayersInputs.Select(CreateInputProvider).ToArray();
            LevelInstance levelInstance = _levelGenerator.GenerateLevel(userInputProviders, levelLayout);

            return levelInstance;
        }

        private IEnumerator StartLevelOncePlayerMoved(LevelInstance level)
        {
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
                UpdateStatisticsAfterLevel(playerTookNoHits);
                
                interLevelFlow.StartAction(GenerateNextLevel);
            }
            else
            {
                interLevelFlow.StartAction(ResetCurrentLevel);
            }
        }

        private void UpdateStatisticsAfterLevel(bool playerTookNoHits)
        {
            _runTracker.IsPerfect &= playerTookNoHits && _runTracker.Deaths == 0;
            if (!_runTracker.HasSkipped)
            {
                int userFacingLevelIndex = _currentLevelIndex + 1;
                _playerStats.UpdateHighestLevel(userFacingLevelIndex, _runTracker.Deaths == 0, _runTracker.IsPerfect);
                
                LevelLayout currentLevel = _levelProgression.LevelLayout[_currentLevelIndex];
                CircumDebug.Log($"Level {currentLevel.name} finished\nRun stats: {_runTracker}\nPlayer stats = {_playerStats}");
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
    }
}