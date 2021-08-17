using Code.Core;
using UnityEngine;

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

        protected override void UpdateInternal()
        {
            base.UpdateInternal();
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
            if (other.IsOrbiter())
            {
                Vector3 direction = (transform.position - other.transform.position).normalized;
                _hitVector += (Vector2)direction * _yeetScale;
            }
        }
    }
}