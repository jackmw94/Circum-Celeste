using Code.Behaviours;
using UnityEngine;

namespace Code.VFX
{
    public class SetShaderPropertyToRandom : MonoBehaviour
    {
        [SerializeField] private MaterialProvider _materialProvider;
        [SerializeField] private string _materialPropertyName;
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;
        
        private void Awake()
        {
            Material mat = _materialProvider.GetMaterial();
            float randomValue = Random.Range(_minValue, _maxValue);
            mat.SetFloat(_materialPropertyName, randomValue);
        }
    }
}