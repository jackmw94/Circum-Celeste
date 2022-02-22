using System.Collections;
using UnityEngine;
using UnityExtras.Code.Optional.EasingFunctions;
using UnityExtras.Core;

namespace Code.VFX
{
    public class BlackHoleImpactEffect : MonoBehaviour
    {
        [SerializeField] private BlackHoleImpactVfxSettings _settings;
        [SerializeField] private MeshRenderer _meshRenderer;

        private IEnumerator Start()
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            
            yield return Utilities.LerpOverTime(0f, 1f, _settings.Duration, f =>
            {
                float easedF = EasingFunctions.EaseInSine(f);
                SetEffectNormalisedTime(easedF);
            });
        }

        private void SetEffectNormalisedTime(float normalisedTime)
        {
            _meshRenderer.material.SetFloat("Magnitude", Mathf.Lerp(_settings.Magnitude, 0f, normalisedTime));
            _meshRenderer.material.SetFloat("RemapY", Mathf.Lerp(_settings.Power, 0f, normalisedTime));
        }
    }
}