using UnityEngine;

namespace Code.Level.Player
{
    public class DebugPlayerInputInitialiser : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PlayerMover _playerMover;

        private void Awake()
        {
            _playerInput.Initialise(new KeyboardInputProvider());
            _playerMover.MovementEnabled = true;
        }
    }
}