using System.Collections;
using Code.Debugging;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.Singletons;

namespace Code.VFX
{
    public class PostProcessingFeedback : SingletonMonoBehaviour<PostProcessingFeedback>
    {
        [SerializeField] private Volume _volume;
        [SerializeField] private AnimationCurve _animationCurve;

        private Vignette _vignette;
        private Coroutine _vignetteCoroutine;

        private void Awake()
        {
            bool vignetteFound = _volume.profile.TryGet(out _vignette);
            
            CircumDebug.Assert(vignetteFound, $"Could not find a vignette override on post processing ({gameObject})");
        }

        public void TriggerVignetteHit()
        {
            if (_vignetteCoroutine != null)
            {
                StopCoroutine(_vignetteCoroutine);
            }
            _vignetteCoroutine = StartCoroutine(VignetteHitCoroutine());
        }

        private IEnumerator VignetteHitCoroutine()
        {
            yield return Utilities.LerpOverTime(0f, 1f, _animationCurve.GetCurveDuration(), f =>
            {
                _vignette.intensity.value = f;
            });
        }
    }
}