using System;
using System.Collections;
using System.Linq;
using Code.Debugging;
using Code.Flow;
using Code.Level.Player;
using UnityEngine;

namespace Code.Level
{
    public class LevelManager : MonoBehaviour
    {
        private enum InputType
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

        private int NextLevelIndex => (_currentLevelIndex + 1) % _levelProgression.LevelLayout.Length;

        public LevelInstance CurrentLevel { get; private set; }

        private void Start()
        {
            CircumDebug.Assert(_playersInputs.Length > 0, "There are no users for this level, probably not what we want..");
            
            InterLevelFlow interLevelFlow = FlowContainer.Instance.InterLevelFlow;
            interLevelFlow.ShowOverlayInstant = true;
            interLevelFlow.ShowNextLevelName = false;
            interLevelFlow.StartAction(null);
            CreateLevel();
        }

        public string GetNextLevelName()
        {
            LevelLayout nextLevel = _levelProgression.LevelLayout[NextLevelIndex];
            return nextLevel.name;
        }

        public string GetCurrentLevelName()
        {
            LevelLayout nextLevel = _levelProgression.LevelLayout[_currentLevelIndex];
            return nextLevel.name;
        }

        public void GenerateNextLevel()
        {
            _currentLevelIndex = NextLevelIndex;
            CreateLevel();
        }

        public void ResetCurrentLevel()
        {
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
            InputProvider[] userInputProviders = _playersInputs.Select(CreateInputProvider).ToArray();
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

        private void OnLevelFinished(bool success)
        {
            InterLevelFlow interLevelFlow = FlowContainer.Instance.InterLevelFlow;
            interLevelFlow.ShowOverlayInstant = false;
            interLevelFlow.ShowNextLevelName = success;
            
            if (success)
            {
                interLevelFlow.StartAction(GenerateNextLevel);
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
    }
}