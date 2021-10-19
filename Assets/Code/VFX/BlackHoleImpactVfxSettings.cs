using UnityEngine;

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
}