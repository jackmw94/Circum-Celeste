using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Behaviours
{
    public class IntermittentJiggle : IntroductionBehaviour
    {
        [SerializeField] private AnimationCurve _jiggleScaleCurve;
        [SerializeField] private float _scaleMultiplier = 1f;

        private float _startTime;

        private void OnEnable()
        {
            _startTime = Time.time;
        }

        private void OnDisable()
        {
            transform.localPosition = Vector3.zero;
        }

        private void Update()
        {
            float animationTime = Time.time - _startTime;
            animationTime = animationTime % _jiggleScaleCurve.GetCurveDuration();
            float jiggleScale = _jiggleScaleCurve.Evaluate(animationTime) * _scaleMultiplier;
            
            // y arguments arbitrary, just there to give us different signals for x and y
            float jiggleX = Mathf.PerlinNoise(Time.time, 100f);
            float jiggleY = Mathf.PerlinNoise(Time.time, 1000f);

            jiggleX = ((jiggleX * 2f) - 1f) * jiggleScale;
            jiggleY = ((jiggleY * 2f) - 1f) * jiggleScale;

            Transform jigglerTransform = transform;
            jigglerTransform.localPosition = new Vector3(jiggleX, jiggleY, jigglerTransform.localPosition.z);
        }
    }
}