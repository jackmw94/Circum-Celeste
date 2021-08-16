using System;
using UnityEngine;

namespace Code.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private PlayerInput _playerInput;
        [Space(15)]
        [SerializeField] private float _speed;

        public bool MovementEnabled { get; set; }
        
        private InputProvider InputProvider => _playerInput.InputProvider;

        private void FixedUpdate()
        {
            _rigidbody.velocity = Vector3.zero;

            // should non-physics movement be performed in regular update
            if (MovementEnabled)
            {
                Vector3 movement = InputProvider.GetMovementInput();
                movement = movement.normalized;
                transform.position += movement * (Time.deltaTime * _speed);
            }
        }
    }
}