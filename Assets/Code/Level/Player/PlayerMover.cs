using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private PlayerInput _playerInput;
        [Space(15)]
        [SerializeField] private float _speed;

        public bool IsMoving => MovementEnabled && InputProvider.GetMovementInput().magnitude > float.Epsilon;
        public bool MovementEnabled { get; set; }
        
        private InputProvider InputProvider => _playerInput.InputProvider;

        private void Awake()
        {
            _speed = RemoteConfigHelper.PlayerSpeed;
        }
        
        private void FixedUpdate()
        {
            _rigidbody.velocity = Vector3.zero;
            
            if (MovementEnabled)
            {
                Vector3 movement = InputProvider.GetMovementInput();
                movement = movement.normalized;
                transform.position += movement * (Time.deltaTime * _speed);
            }
        }
    }
}