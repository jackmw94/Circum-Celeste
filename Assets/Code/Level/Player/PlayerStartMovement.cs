using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level.Player
{
    public class PlayerStartMovement : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _magnitude;

        private float _angle = 0f;
    
        private void Update()
        {
            _angle += _speed * Time.deltaTime;
            transform.position = Vector2.up.Rotate(_angle) * _magnitude;
        }
    }
}