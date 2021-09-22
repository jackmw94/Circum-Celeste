using UnityEngine;

namespace Code.Debugging
{
    public class PositionBetweenTransforms : MonoBehaviour
    {
        [SerializeField] private Transform _point1;
        [SerializeField] private Transform _point2;
        [Space(15)]
        [SerializeField] private float _normalisedPointBetween = 0.5f;

        public void Setup(Transform point1, Transform point2, float? pointBetween = null)
        {
            _point1 = point1;
            _point2 = point2;
            _normalisedPointBetween = pointBetween ?? _normalisedPointBetween;
        }

        private void Update()
        {
            Vector3 point1Position = _point1.position;
            Vector3 point2Position = _point2.position;
            transform.position = point1Position + (point2Position - point1Position) * _normalisedPointBetween;
        }
    }
}