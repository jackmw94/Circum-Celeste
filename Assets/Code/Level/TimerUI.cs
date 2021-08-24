using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class TimerUI : MonoBehaviour
    {
        private float _duration;
        private float _startTime;
        private bool _running = false;
        
        private void Awake()
        {
            ResetTimer();
        }

        public void StartTimer(float duration)
        {
            _startTime = Time.time;
            _duration = duration;
            _running = true;
        }

        public void ResetTimer()
        {
            SetTimerProgress(0f);
            _running = false;
        }

        private void Update()
        {
            if (!_running)
            {
                return;
            }
            
            float timeSinceStart = Time.time - _startTime;

            float safeDuration = Mathf.Max(_duration, float.Epsilon);
            float progress = timeSinceStart / safeDuration;
            
            SetTimerProgress(progress);
            
            if (progress >= 1f)
            {
                _running = false;
            }
        }

        private void SetTimerProgress(float progress)
        {
            float clampedProgress = Mathf.Clamp01(progress);
            Transform timerTransform = transform;
            timerTransform.localScale = timerTransform.localScale.ModifyVectorElement(0, clampedProgress);
        }
    }
}