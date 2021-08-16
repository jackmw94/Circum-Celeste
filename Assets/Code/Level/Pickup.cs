using System;
using Code.VFX;
using UnityEngine;

namespace Code.Level
{
    public class Pickup : MonoBehaviour
    {
        private const float DestroyVfxMaxFinishTime = 5f;
        
        [SerializeField] private PickupType _pickupType;

        private int _orbiterLayer;
        private int _playerLayer;

        public static Action PickupCollected = () => { };

        private void Awake()
        {
            _orbiterLayer = LayerMask.NameToLayer($"Orbiter");
            _playerLayer = LayerMask.NameToLayer($"Player");
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
            int otherObjectLayer = other.layer;
            bool collectedByOrbiter = otherObjectLayer == _orbiterLayer && _pickupType.HasFlag(PickupType.OrbiterCollect);
            bool collectedByPlayer = otherObjectLayer == _playerLayer && _pickupType.HasFlag(PickupType.PlayerCollect);
        
            if (collectedByOrbiter || collectedByPlayer)
            {
                PickupCollected();
                Destroy(gameObject);

                Vector3 transformPosition = transform.position;
                Vector3 direction = transformPosition - other.transform.position;
                VfxType vfxType = PickupTypeToVfxCollectedType(_pickupType);
                VfxManager.Instance.SpawnVfx(vfxType, transformPosition, direction);
            }
        }

        private static VfxType PickupTypeToVfxCollectedType(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.OrbiterCollect:
                    return VfxType.OrbiterPickupCollected;
                case PickupType.PlayerCollect:
                    return VfxType.PlayerPickupCollected;
            }

            Debug.LogError($"Could not get vfx type for pickup type {pickupType}");
            return VfxType.None;
        }
    }
}