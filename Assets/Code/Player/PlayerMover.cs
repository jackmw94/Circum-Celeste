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

        private InputProvider InputProvider => _playerInput.InputProvider;
        
        private void FixedUpdate()
        {
            _rigidbody.velocity = Vector3.zero;
            
            Vector3 movement = InputProvider.GetMovementInput();

            movement = movement.normalized;
        
            transform.position += movement * (Time.deltaTime * _speed);
        } 
    }
}