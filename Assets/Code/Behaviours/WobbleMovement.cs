using UnityEngine;
using UnityExtras.Code.Optional.EasingFunctions;

namespace Code.Behaviours
{
    public class WobbleMovement : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetLocalPosition;
        [SerializeField] private EasingFunctions.EasingType _easing;
        [SerializeField] private float _normalisedOffset;
        [SerializeField] private float _cycleDuration;

        private Vector3 _defaultPosition;
        
        private void Awake()
        {
            _defaultPosition = transform.localPosition;
        }
        
        private void Update()
        {
            float time = Time.time + _normalisedOffset * _cycleDuration;
            float cycleTime = (time % _cycleDuration) / _cycleDuration;

            float triangle = cycleTime < 0.5f ? cycleTime * 2f : 2f - cycleTime * 2f;
            float eased = EasingFunctions.ConvertLinearToEased(_easing, EasingFunctions.EasingDirection.InAndOut, triangle);

            transform.localPosition = Vector3.Lerp(_defaultPosition, _targetLocalPosition, eased);
        }
    }
}