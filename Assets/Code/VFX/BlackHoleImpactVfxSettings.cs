using System.Collections;
using UnityEngine;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.EasingFunctions;

namespace Code.VFX
{
    [CreateAssetMenu(menuName = "Create BlackHoleImpactVfxSettings", fileName = "BlackHoleImpactVfxSettings", order = 0)]
    public class BlackHoleImpactVfxSettings : ScriptableObject
    {
        [SerializeField] private float _power;
        [SerializeField] private float _magnitude;
        [SerializeField] private float _duration;

        public float Power => _power;
        public float Magnitude => _magnitude;
        public float Duration => _duration;
    }

    public class BlackHoleImpactEffect : MonoBehaviour
    {
        [SerializeField] private BlackHoleImpactVfxSettings _settings;
        [SerializeField] private MeshRenderer _meshRenderer;

        private IEnumerator Start()
        {
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