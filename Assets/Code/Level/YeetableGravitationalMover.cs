using Code.Core;
using Code.Juice;
using Code.VFX;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    /// <summary>
    /// Handles throwing off into the distance if hit by an orbiter
    /// </summary>
    public class YeetableGravitationalMover : GravitationalMover
    {
        [SerializeField] private float _yeetScale = 1f;
        [SerializeField] private float _yeetInertia = 0.95f;
        
        private Vector2 _hitVector = Vector2.zero;

        protected override void FixedUpdateInternal()
        {
            base.FixedUpdateInternal();
            transform.Translate(_hitVector);
            _hitVector *= _yeetInertia;
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other.gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            HandleCollision(other.gameObject);
        }

        private void HandleCollision(GameObject other)
        {
            if (!other.IsOrbiter())
            {
                return;
            }
            
            // got yeet
            Vector3 transformPosition = transform.position;
            Vector3 direction = (transformPosition - other.transform.position).normalized;
            _hitVector += (Vector2)direction * _yeetScale;

            Feedbacks.Instance.TriggerFeedback(Feedbacks.FeedbackType.HitEnemy);
            VfxManager.Instance.SpawnVfx(VfxType.BlackHoleHit, transformPosition.ModifyVectorElement(2, 20f));
        }
    }
}