using UnityEngine;
using UnityEngine.VFX;

namespace Code.VFX
{
    public class FollowParticles : MonoBehaviour
    {
        [SerializeField] private Transform _particlesScaleTransform;
        [SerializeField] private GameObject _particlesPrefab;
        [SerializeField] private string _positionProperty;
        [SerializeField] private string _scaleProperty;
        [SerializeField] private Vector3 _offset = Vector3.back;
        
        private VisualEffect _particlesInstance;
        private int _positionPropertyId;
        private int _scalePropertyId;

        private void Awake()
        {
            _positionPropertyId = Shader.PropertyToID(_positionProperty);
            _scalePropertyId = Shader.PropertyToID(_scaleProperty);
        }

        private void OnEnable()
        {
            GameObject particlesObj = Instantiate(_particlesPrefab, Vector3.zero, Quaternion.identity);
            _particlesInstance = particlesObj.GetComponent<VisualEffect>();
        }

        private void Update()
        {
            _particlesInstance.SetVector3(_positionPropertyId, transform.position + _offset);
            _particlesInstance.SetVector3(_scalePropertyId, _particlesScaleTransform.localScale);
        }

        private void OnDisable()
        {
            if (_particlesInstance)
            {
                Destroy(_particlesInstance.gameObject);
            }
        }
    }
}