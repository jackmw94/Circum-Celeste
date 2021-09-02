using System.Diagnostics;
using Code.Core;
using Code.VFX;
using UnityEngine;

namespace Code.Level
{
    public class Hazard : Collectable
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
        
        protected override void CollectableCollected(Vector3 hitFrom)
        {
            gameObject.SetActive(false);

            Vector3 transformPosition = transform.position;
            Vector3 direction = transformPosition - hitFrom;
            VfxManager.Instance.SpawnVfx(VfxType.HazardHit, transformPosition, direction);
        }
    }
}