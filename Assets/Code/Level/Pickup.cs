using System.Diagnostics;
using Code.Core;
using Code.Juice;
using Code.VFX;
using UnityEngine;

namespace Code.Level
{
    public class Pickup : Collectable
    {
        [SerializeField] private SphereCollider _collider;
        
        public bool IsCollected { get; private set; }

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
            _collider.radius = RemoteConfigHelper.PickupColliderSize;
        }
        
        protected override void CollectableCollected(GameObject collectedBy)
        {
            gameObject.SetActive(false);
            
            Feedbacks.Instance.TriggerFeedback(Feedbacks.FeedbackType.CollectedPickup);

            Vector3 transformPosition = transform.position;
            Vector3 direction = transformPosition - collectedBy.transform.position;
            VfxManager.Instance.SpawnVfx(VfxType.PickupCollected, transformPosition, direction);
        }
    }
}