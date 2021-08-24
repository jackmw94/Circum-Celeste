using System.Diagnostics;
using Code.Core;
using Code.VFX;
using UnityEngine;

namespace Code.Level
{
    public class Hazard : MonoBehaviour
    {
        [SerializeField] private SphereCollider _collider;
        
        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_collider)
            {
                _collider = GetComponent<SphereCollider>();
            }
        }

        private void Awake()
        {
            _collider.radius = RemoteConfigHelper.HazardColliderSize;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            HandleCollision(other.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other.gameObject);
        }

        private void HandleCollision(GameObject other)
        {
            if (other.IsOrbiter())
            {
                HazardHit(other);
            }
        }

        private void HazardHit(GameObject hitBy)
        {
            gameObject.SetActive(false);
            
            Vector3 transformPosition = transform.position;
            Vector3 direction = transformPosition - hitBy.transform.position;
            VfxManager.Instance.SpawnVfx(VfxType.HazardHit, transformPosition, direction);
        }
    }
}