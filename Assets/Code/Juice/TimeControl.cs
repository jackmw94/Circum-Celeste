using System;
using UnityEngine;

namespace Code.Juice
{
    public class TimeControl : MonoBehaviour
    {
        [Serializable]
        public class TimeControlFeedback
        {
            [SerializeField] private bool _enabled;
            [SerializeField] private float _duration;
            [SerializeField] private float _timeScale;

            public bool Enabled => _enabled;
            public float Duration => _duration;
            public float TimeScale => _timeScale;
        }
        
        private class TimeState
        {
            private readonly TimeControlFeedback _timeControlFeedback;
            private readonly float _unscaledStartTime;
            
            private float TimeStateTime => Time.unscaledTime - _unscaledStartTime;
            public float TimeRemaining => _timeControlFeedback.Duration - TimeStateTime;
            public float TimeScale => _timeControlFeedback.TimeScale;
            
            public TimeState(TimeControlFeedback timeControlFeedback)
            {
                _timeControlFeedback = timeControlFeedback;
                _unscaledStartTime = Time.unscaledTime;
            }
        }

        private TimeState _currentTimeState = null;

        private void Update()
        {
            if (_currentTimeState == null)
            {
                Time.timeScale = 1f;
                return;
            }

            if (_currentTimeState.TimeRemaining <= 0f)
            {
                _currentTimeState = null;
                Time.timeScale = 1f;
                return;
            }

            Time.timeScale = _currentTimeState.TimeScale;
        }

        public void AddTimeState(TimeControlFeedback timeScaleFeedback)
        {
            if (timeScaleFeedback.Enabled)
            {
                // overwrites previous
                _currentTimeState = new TimeState(timeScaleFeedback);
            }
        }
    }
}