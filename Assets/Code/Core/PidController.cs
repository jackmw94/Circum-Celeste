using System;
using UnityEngine;

namespace Code.Core
{
    [Serializable]
    public class PidController
    {
        [SerializeField] private PidProperties _pidProperties;
    
        private float _maxIntegralOffset;
    
        private float _integral;
        private float _lastError;

        public void SetMaxIntegralOffset(float maxIntegralOffset)
        {
            _maxIntegralOffset = maxIntegralOffset;
        }
    
        public float GetPidDiff(float target, float current, float frameTime)
        {
            float proportionalError = target - current;
            _integral += proportionalError * frameTime;
            _integral = Mathf.Clamp(_integral, -(_pidProperties.MaxIntegral + _maxIntegralOffset), (_pidProperties.MaxIntegral + _maxIntegralOffset));

            float derivative = (proportionalError - _lastError) / frameTime;
            _lastError = proportionalError;

            return (proportionalError * _pidProperties.ProportionalFactor) + (_integral * _pidProperties.MaxIntegral) + (derivative * _pidProperties.DerivativeFactor);
        }

        public void ResetPid()
        {
            _lastError = 0f;
            _integral = 0f;
        }
    }
}