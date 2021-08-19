using UnityEngine;

namespace Code.VFX
{
    public class Expand : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _animationCurve;

        private float _startTime = 0f;

        private void Awake()
        {
            _startTime = Time.time;
        }
        
        private void Update()
        {
            float time = Time.time - _startTime;
            float expansion = _animationCurve.Evaluate(time);
            transform.localScale = Vector3.one * expansion;
        }
    }
}