using Code.Core;
using Code.VFX;
using UnityEngine;

namespace Code.Level
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private PickupType _pickupType;

        public bool IsCollected { get; private set; }
        
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
            bool collectedByOrbiter = other.IsOrbiter() && _pickupType.HasFlag(PickupType.OrbiterCollect);
            bool collectedByPlayer = other.IsPlayer() && _pickupType.HasFlag(PickupType.PlayerCollect);
        
            if (collectedByOrbiter || collectedByPlayer)
            {
                Collected(other);
            }
        }

        private void Collected(GameObject collectedBy)
        {
            IsCollected = true;
            gameObject.SetActive(false);

            Vector3 transformPosition = transform.position;
            Vector3 direction = transformPosition - collectedBy.transform.position;
            VfxType vfxType = PickupTypeToVfxCollectedType(_pickupType);
            VfxManager.Instance.SpawnVfx(vfxType, transformPosition, direction);
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