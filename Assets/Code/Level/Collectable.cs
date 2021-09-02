using Code.Core;
using UnityEngine;

namespace Code.Level
{
    public abstract class Collectable : LevelElement
    {
        public bool IsCollected { get; private set; } = false;
        
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
            if (!CanObjectCollect(other))
            {
                return;
            }

            IsCollected = true;
            CollectableCollected(other.transform.position);
        }

        protected virtual bool CanObjectCollect(GameObject other)
        {
            return other.IsOrbiter();
        }
        
        protected abstract void CollectableCollected(Vector3 hitFrom);
    }
}