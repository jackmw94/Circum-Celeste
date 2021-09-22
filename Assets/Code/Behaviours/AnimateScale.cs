using UnityEngine;

namespace Code.Behaviours
{
    public class AnimateScale : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _normalisedAnimationCurve;
        [SerializeField] private float _duration;

        private float _startTime;

        private void OnEnable()
        {
            _startTime = Time.time;
            
            float localScale = _normalisedAnimationCurve.Evaluate(0f);
            transform.localScale = Vector3.one * localScale;
        }

        private void Update()
        {
            float timeSinceStart = Time.time - _startTime;
            float cycleTime = timeSinceStart % _duration;
            float normalisedCycleTime = cycleTime / _duration;
            float localScale = _normalisedAnimationCurve.Evaluate(normalisedCycleTime);
            transform.localScale = Vector3.one * localScale;
        }
    }
}