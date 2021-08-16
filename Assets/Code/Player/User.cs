using System;
using UnityEngine;

namespace Code.Player
{
    [Serializable]
    public class User
    {
        public enum InputType
        {
            Keyboard,
            Mouse,
            Controller,
            UI
        }

        [SerializeField] private Player _player;
        [SerializeField] private InputType _inputType;

        public Player Player => _player;
        public InputProvider InputProvider => CreateInputProvider(_inputType);

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