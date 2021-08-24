using UnityEngine;
using UnityEngine.VFX;

namespace Code.VFX
{
    public class FollowParticles : MonoBehaviour
    {
        [SerializeField] private GameObject _particlesPrefab;
        [SerializeField] private string _positionProperty;
        
        private VisualEffect _particlesInstance;

        private void OnEnable()
        {
            GameObject particlesObj = Instantiate(_particlesPrefab, Vector3.zero, Quaternion.identity);
            _particlesInstance = particlesObj.GetComponent<VisualEffect>();
        }

        private void Update()
        {
            _particlesInstance.SetVector3(_positionProperty, transform.position);
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