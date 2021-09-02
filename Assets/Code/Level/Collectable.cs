using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Level.Player;
using UnityEngine;

namespace Code.Level
{
    public abstract class Collectable : LevelElement
    {
        private int _collectableIndex = -1;
        private readonly HashSet<GameObject> _collidedCollectorObjects = new HashSet<GameObject>();
        
        public bool CanBeCollected { get; private set; } = false;

        public void Initialise(int collectableIndex)
        {
            _collectableIndex = collectableIndex;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            HandleCollision(other.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other.gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            HandleCollisionEnd(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            HandleCollisionEnd(other.gameObject);
        }

        private void HandleCollisionEnd(GameObject other)
        {
            if (_collidedCollectorObjects.Contains(other.gameObject))
            {
                _collidedCollectorObjects.Remove(other.gameObject);
            }
        }

        private void HandleCollision(GameObject other)
        {
            if (!CanObjectCollect(other))
            {
                return;
            }

            _collidedCollectorObjects.Add(other);
            TriggerCollection(other.transform.position);
        }

        protected virtual bool CanObjectCollect(GameObject other)
        {
            return other.IsOrbiter();
        }

        public override void Tick(LevelRecordFrameData frameReplay)
        {
            base.Tick(frameReplay);
            if (frameReplay == null)
            {
                if (_collidedCollectorObjects.Count > 0)
                {
                    TriggerCollection(_collidedCollectorObjects.First().transform.position);
                }
            }
            else
            {
                if (DoesReplayCollect(frameReplay, _collectableIndex, out Vector3 hitFrom))
                {
                    TriggerCollection(hitFrom);
                }
            }
        }

        public void TriggerCollection(Vector3 hitFrom)
        {
            CanBeCollected = true;
            CollectableCollected(hitFrom);
        }

        protected abstract void CollectableCollected(Vector3 hitFrom);

        protected abstract bool DoesReplayCollect(LevelRecordFrameData frameReplay, int index, out Vector3 hitFrom);
    }
}