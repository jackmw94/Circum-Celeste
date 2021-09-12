using System;
using System.Collections;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Behaviours
{
    public abstract class AnimateShaderPropertyBase : MonoBehaviour
    {
        [SerializeField] private string _propertyName;
        [SerializeField] private float _animationDuration;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private bool _setFirstFrameOnAwake = false;

        private Action _onComplete = null;
        private Coroutine _animationCoroutine = null;
        private Material _material;
        private int _propertyId;

        private void Awake()
        {
            _propertyId = Shader.PropertyToID(_propertyName);
            _material = GetMaterial();
            if (_setFirstFrameOnAwake)
            {
                SetValue(0f);
            }
        }

        protected abstract Material GetMaterial();

        [ContextMenu(nameof(TriggerAnimation))]
        public void TriggerAnimation() => TriggerAnimation(null);

        public void TriggerAnimation(Action onComplete)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _onComplete = onComplete;
            _animationCoroutine = StartCoroutine(RunAnimationCoroutine());
        }

        private IEnumerator RunAnimationCoroutine()
        {
            yield return Utilities.LerpOverTime(0f, 1f, _animationDuration, SetValue);
            _onComplete?.Invoke();
        }

        private void SetValue(float normalisedValue)
        {
            float curvedValue = _animationCurve.Evaluate(normalisedValue);
            _material.SetFloat(_propertyId, curvedValue);
        }
    }
}