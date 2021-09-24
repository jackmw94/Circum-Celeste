using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace Code.VFX
{
    public class OrbiterVfx : MonoBehaviour
    {
        [FormerlySerializedAs("_particlesPrefab")]
        [SerializeField] private GameObject _vfxPrefab;

        [SerializeField] private Transform _particlesScaleTransform;
        [SerializeField] private string _positionProperty;
        [SerializeField] private string _scaleProperty;
        [SerializeField] private Vector3 _offset = Vector3.back;

        private GameObject _particlesRoot;
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
            _particlesRoot = Instantiate(_vfxPrefab, Vector3.zero, Quaternion.identity);
            QualityLevelControlledObjects qualityLevelControlledObjects = _particlesRoot.GetComponent<QualityLevelControlledObjects>();
            
            if (qualityLevelControlledObjects)
            {
                GameObject activeObjectForCurrentQualityLevel = qualityLevelControlledObjects.GetActiveObjectForCurrentQualityLevel();
                _particlesInstance = activeObjectForCurrentQualityLevel.GetComponent<VisualEffect>();
            }
            else
            {
                _particlesInstance = _particlesRoot.GetComponent<VisualEffect>();
            }
        }

        private void Update()
        {
            if (!_particlesInstance)
            {
                Transform particlesRootTransform = _particlesRoot.transform;
                particlesRootTransform.position = transform.position + _offset;
                particlesRootTransform.localScale = _particlesScaleTransform.localScale;
                return;
            }
            
            _particlesInstance.SetVector3(_positionPropertyId, transform.position + _offset);
            _particlesInstance.SetVector3(_scalePropertyId, _particlesScaleTransform.localScale);
        }

        private void OnDisable()
        {
            if (_particlesRoot)
            {
                Destroy(_particlesRoot);
            }
        }
    }
}