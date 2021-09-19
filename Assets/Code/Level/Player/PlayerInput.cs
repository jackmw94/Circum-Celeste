using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerInput : MonoBehaviour
    {
        private InputProvider _inputProvider;

        public InputProvider InputProvider => _inputProvider;
        
        public void Initialise(InputProvider inputProvider)
        {
            _inputProvider = inputProvider;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(EditorKeyCodeBindings.SwitchToKeyboardInput)) _inputProvider = InputProvider.CreateInputProvider<KeyboardInputProvider>();
            if (Input.GetKeyDown(EditorKeyCodeBindings.SwitchToControllerInput)) _inputProvider = InputProvider.CreateInputProvider<ControllerInputProvider>();
            if (Input.GetKeyDown(EditorKeyCodeBindings.SwitchToMouseInput)) _inputProvider = InputProvider.CreateInputProvider<MouseInputProvider>();
            if (Input.GetKeyDown(EditorKeyCodeBindings.SwitchToUIInput)) _inputProvider = InputProvider.CreateInputProvider<UIInputProvider>();
        }
#endif
    }
}