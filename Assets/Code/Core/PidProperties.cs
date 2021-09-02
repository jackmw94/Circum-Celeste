using UnityEngine;

namespace Code.Core
{
    [CreateAssetMenu(menuName = "Create PidProperties", fileName = "PidProperties", order = 0)]
    public class PidProperties : ScriptableObject
    {
        [SerializeField] private float _proportionalFactor;
        [SerializeField] private float _maxIntegral;
        [SerializeField] private float _derivativeFactor;
    
        public float ProportionalFactor => _proportionalFactor;
        public float MaxIntegral => _maxIntegral;
        public float DerivativeFactor => _derivativeFactor;

        public void SetPIDValues(float p, float i, float d)
        {
            _proportionalFactor = p;
            _maxIntegral = i;
            _derivativeFactor = d;
        }
    }
}