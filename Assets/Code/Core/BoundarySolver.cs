using UnityEngine;

namespace Code.Core
{
    public class BoundarySolver : MonoBehaviour
    {
        [SerializeField] private float _recoveryOffsetDistance = 0.25f;
        [SerializeField] private Bounds _bounds;

        private void LateUpdate()
        {
            Vector3 currentPosition = transform.position;
            if (_bounds.Contains(currentPosition))
            {
                return;
            }
            
            // is outside game bounds
            Vector3 withinBoundsPosition = _bounds.ClosestPoint(transform.position);
            Vector3 recoveryOffset = (withinBoundsPosition - currentPosition).normalized * _recoveryOffsetDistance;
            transform.position = withinBoundsPosition + recoveryOffset;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }
    }
}