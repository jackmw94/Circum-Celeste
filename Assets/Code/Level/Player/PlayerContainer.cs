using System;
using System.Linq;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerContainer : MonoBehaviour, IValidateable
    {
        private enum InputType
        {
            Keyboard,
            Mouse,
            Controller,
            UI
        }
        
        [SerializeField] private InputType[] _playersInputs;
        [SerializeField] private InputType[] _editorPlayersInputs;

        private InputType[] _activePlayerInputs = null;
        private InputType[] PlayerInputs => _activePlayerInputs ??= Application.isEditor ? _editorPlayersInputs : _playersInputs;

        private void Awake()
        {
            CircumDebug.Assert(_playersInputs.Length > 0, "There are no platform players defined in player container. Probably not what we want..");
            CircumDebug.Assert(_editorPlayersInputs.Length > 0, "There are no editor players defined in player container. Probably not what we want..");
        }

        public InputProvider[] GetPlayerInputProviders()
        {
            return PlayerInputs.Select(CreateInputProvider).ToArray();
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