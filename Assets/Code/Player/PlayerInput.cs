using UnityEngine;

namespace Code.Player
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
            if (Input.GetKeyDown(KeyCode.Alpha1)) _inputProvider = InputProvider.CreateInputProvider<KeyboardInputProvider>();
            if (Input.GetKeyDown(KeyCode.Alpha2)) _inputProvider = InputProvider.CreateInputProvider<MouseInputProvider>();
            if (Input.GetKeyDown(KeyCode.Alpha3)) _inputProvider = InputProvider.CreateInputProvider<ControllerInputProvider>();
            if (Input.GetKeyDown(KeyCode.Alpha4)) _inputProvider = InputProvider.CreateInputProvider<UIInputProvider>();
        }
#endif
    }
}