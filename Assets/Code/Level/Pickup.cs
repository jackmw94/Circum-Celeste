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
        [SerializeField] private Transform _visualsTransform;
        

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
            _visualsTransform.Rotate(Vector3.forward, Random.Range(0f, 360f));
        }
        
        protected override void CollectableCollected(Vector3 hitFrom)
        {
            gameObject.SetActive(false);
            
            Feedbacks.Instance.TriggerFeedback(Feedbacks.FeedbackType.CollectedPickup);

            Vector3 transformPosition = transform.position;
            Vector3 direction = transformPosition - hitFrom;
            VfxManager.Instance.SpawnVfx(VfxType.PickupCollected, transformPosition, direction);
        }
    }
}