using UnityEngine;

namespace Code.Behaviours
{
    public class SimpleMove : MonoBehaviour
    {
        [SerializeField] private Vector3 _localTargetPosition;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _destroyOnFinish = false;

        private float _startTime = 0f;
        private Vector3 _startPosition;

        private float ActiveTime => Time.time - _startTime;
        
        private void OnEnable()
        {
            _startTime = Time.time;
            _startPosition = transform.localPosition;
        }

        private void Update()
        {
            if (ActiveTime >= _duration)
            {
                transform.localPosition = _localTargetPosition;

                if (_destroyOnFinish)
                {
                    Destroy(this);
                }
                
                return;
            }

            float normalisedActiveTime = ActiveTime / Mathf.Max(_duration, float.Epsilon);
            Vector3 currentLocalPosition = Vector3.Lerp(_startPosition, _localTargetPosition, normalisedActiveTime);
            transform.localPosition = currentLocalPosition;
        }
    }
}