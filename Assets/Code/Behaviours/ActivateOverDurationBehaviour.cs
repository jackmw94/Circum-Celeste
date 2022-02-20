using System;
using System.Collections;
using UnityEngine;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.EasingFunctions;

namespace Code.Behaviours
{
    public abstract class ActivateOverDurationBehaviour : MonoBehaviour
    {
        [SerializeField] private float _startingValue = 0f;
        [SerializeField] private float _duration;
        [SerializeField] private EasingFunctions.EasingType _easingType = EasingFunctions.EasingType.Linear;
        [SerializeField] private EasingFunctions.EasingDirection _easingDirection = EasingFunctions.EasingDirection.InAndOut;

        private Coroutine _activateCoroutine;
        private float _currentActivatedAmount = 0f;

        public float CurrentActivatedAmount => _currentActivatedAmount;

        protected virtual void Awake()
        {
            SetActivatedAmount(_startingValue);
        }

        public void ActivateDeactivate(bool activate, Action onComplete = null)
        {
            OnActivateDeactivate(activate);
            this.RestartCoroutine(ref _activateCoroutine, ActivateDeactivateCoroutine(activate, onComplete));
        }

        private IEnumerator ActivateDeactivateCoroutine(bool activate, Action onComplete)
        {
            float startingAmount = _currentActivatedAmount;
            float targetAmount = activate ? 1f : 0f;
            yield return Utilities.LerpOverTime(startingAmount, targetAmount, _duration, f =>
            {
                _currentActivatedAmount = f;
                float easedF = EasingFunctions.ConvertLinearToEased(_easingType, _easingDirection, f);
                SetActivatedAmount(easedF);
            });
            onComplete?.Invoke();
        }

        protected virtual void OnActivateDeactivate(bool activate){}
        
        protected abstract void SetActivatedAmount(float amount);
        
#if UNITY_EDITOR
        [ContextMenu(nameof(DebugActivate))]
        private void DebugActivate() => ActivateDeactivate(true);
        
        [ContextMenu(nameof(DebugDeactivate))]
        private void DebugDeactivate() => ActivateDeactivate(false);
#endif
    }
}