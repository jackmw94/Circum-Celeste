using UnityEngine;

namespace Code.Core
{
    public class OrbiterMover : Mover
    {
        [SerializeField] protected Transform _target;
        
        public void SetTarget(Transform target)
        {
            _target = target;
        }
        
        public virtual void SetupGridSize(int gridSize){}
    }
}