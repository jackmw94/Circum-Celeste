using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level.Player
{
    public class PlayerStartMovement : MonoBehaviour
    {
        [SerializeField] private Transform _orbiter;
        [SerializeField] private float _speed;
        [SerializeField] private float _magnitude;
        [SerializeField] private Vector3 _defaultPosition;

        private float _angle = 0f;

        private void OnEnable()
        {
            _angle = 0f;
            transform.position = GetPositionFromAngle(_angle);
            _orbiter.position = GetPositionFromAngle(_angle + 180f);
        }

        private void Update()
        {
            _angle += _speed * Time.deltaTime;
            transform.position = GetPositionFromAngle(_angle);
        }

        private Vector3 GetPositionFromAngle(float angle)
        {
            Vector2 position2D = Vector2.up.Rotate(angle);
            Vector3 scaledPosition = new Vector3(position2D.x, position2D.y) * _magnitude;
            return _defaultPosition + scaledPosition;
        }
    }
}