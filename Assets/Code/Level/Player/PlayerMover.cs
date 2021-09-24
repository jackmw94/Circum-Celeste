using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerMover : Mover
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

            if (!MovementEnabled || InputProvider == null)
            {
                return;
            }
            
            Vector3 movement = InputProvider.GetMovementInput();
            if (movement.magnitude > 1)
            {
                movement = movement.normalized;
            }

            float movementScale = Time.deltaTime * _speed * MovementSizeScaler;
            transform.Translate(movement * movementScale, Space.World);
        }
    }
}