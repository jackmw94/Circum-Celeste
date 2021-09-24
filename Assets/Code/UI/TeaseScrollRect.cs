using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class TeaseScrollRect : MonoBehaviour, IBeginDragHandler
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private bool _isVertical = true;
        [SerializeField] private bool _teaseInPositiveDirection = true;
        [Space(15)]
        [SerializeField] private float _normalisedMagnitude = 0.1f;
        [SerializeField] private AnimationCurve _normalisedAnimationCurve;
        [SerializeField] private float _cycleDuration = 4f;

        private float _startTime = 0f;
        private float _defaultNormalisedValue = 0f;
        
        public void OnEnable()
        {
            _startTime = Time.time;
            _defaultNormalisedValue = _isVertical ? _scrollRect.normalizedPosition.y : _scrollRect.normalizedPosition.x;
        }
        
        private void Update()
        {
            float teaseTime = Time.time - _startTime;
            float cycleTime = (teaseTime / _cycleDuration) % 1f;
            float animationCurveValue = _normalisedAnimationCurve.Evaluate(cycleTime);
            float scaledAnimatedValue = animationCurveValue * _normalisedMagnitude;
            
            Vector2 currentNormalisedPosition = _scrollRect.normalizedPosition;
            float alteredAxis = _teaseInPositiveDirection ? _defaultNormalisedValue + scaledAnimatedValue : _defaultNormalisedValue - scaledAnimatedValue;

            if (_isVertical)
            {
                currentNormalisedPosition.y = alteredAxis;
            }
            else
            {
                currentNormalisedPosition.x = alteredAxis;
            }

            _scrollRect.normalizedPosition = currentNormalisedPosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            enabled = false;
        }
    }
}