using System;
using System.Collections;
using System.Linq;
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
        
        [SerializeField] private LevelLayout _debugLevelLayout;
        [SerializeField] private LevelProgression _levelProgression;
        [Space(15)]
        [SerializeField] private InputType[] _playersInputs;
        [Space(15)]
        [SerializeField] private LevelGenerator _levelGenerator;
        
        private void Start()
        {
            // todo: move player code out of here:
            Debug.Assert(_playersInputs.Length > 0, "There are no users for this level, probably not what we want..");
            CreateLevel();
        }

        private void CreateLevel()
        {
            // todo: replace this with progress / requested level
#if UNITY_EDITOR
            if (_debugLevelLayout)
            {
                LevelInstance debugLevelInstance = GenerateDebugLevel();
                StartCoroutine(StartLevelOncePlayerMoved(debugLevelInstance));
                return;
            }
#endif

            LevelLayout levelLayout = _levelProgression.LevelLayout[0];
            LevelInstance levelInstance = GenerateLevel(levelLayout);
            StartCoroutine(StartLevelOncePlayerMoved(levelInstance));
        }

        [ContextMenu(nameof(GenerateDebugLevel))]
        private LevelInstance GenerateDebugLevel()
        {
            return GenerateLevel(_debugLevelLayout);
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
            Debug.Log("Level started");
            level.StartLevel();
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