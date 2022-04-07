using System.Collections;
using System.Diagnostics;
using Code.Debugging;
using UnityEngine;
using UnityEngine.Rendering;
using UnityExtras.Core;

namespace Code.VFX
{
    public abstract class AnimatePostProcessingBase<T> : MonoBehaviour where T : VolumeComponent
    {
        [SerializeField] private Volume _volume;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private float _duration = 1f;

        private T _volumeComponent;
        private Coroutine _triggerAnimationCoroutine = null;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_volume)
            {
                _volume = GetComponent<Volume>();
            }
        }
        
        private void Awake()
        {
            bool volumeComponentFound = _volume.profile.TryGet(out _volumeComponent);
            CircumDebug.Assert(volumeComponentFound, $"Could not find a {typeof(T)} volume component on post processing ({gameObject})");
        }
        
        [ContextMenu(nameof(TriggerAnimation))]
        public void TriggerAnimation()
        {
            if (_triggerAnimationCoroutine != null)
            {
                StopCoroutine(_triggerAnimationCoroutine);
            }
            _triggerAnimationCoroutine = StartCoroutine(AnimatePostProcessingCoroutine());
        }

        private IEnumerator AnimatePostProcessingCoroutine()
        {
            yield return Utilities.LerpOverTime(0f, 1f, _duration, f =>
            {
                f = _animationCurve.Evaluate(f);
                SetValue(_volumeComponent, f);
            });
        }

        protected abstract void SetValue(T component, float value);
    }
}