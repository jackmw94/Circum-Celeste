using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerMover : LevelPlayBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private PlayerInput _playerInput;
        [Space(15)]
        [SerializeField] private float _speed;

        public bool IsMoving => MovementEnabled && InputProvider.GetMovementInput(transform.position).magnitude > float.Epsilon;
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
                Vector3 position = transform.position;
                
                Vector3 movement = InputProvider.GetMovementInput(position);
                if (movement.magnitude > 1)
                {
                    movement = movement.normalized;
                }
                position += movement * (Time.deltaTime * _speed);
                
                transform.position = position;
            }
        }
    }
}