using Code.Level.Player;
using UnityEngine;

namespace Code.Core
{
    public class GravitationalMover : LevelPlayBehaviour
    {
        [SerializeField] protected PidController _xPidController;
        [SerializeField] protected PidController _yPidController;
        [Space(15)]
        [SerializeField] protected Transform _target;
        
        private void FixedUpdate()
        {
            if (!_target)
            {
                return;
            }
            
            FixedUpdateInternal();
        }

        protected virtual void FixedUpdateInternal()
        {
            Vector3 movement = GetMovement(_target.position, transform, Time.fixedDeltaTime);
            transform.Translate(movement, Space.World);
        }

        protected virtual Vector3 GetMovement(Vector3 targetPosition, Transform mover, float frameTime)
        {
            return GetPidDiff(targetPosition, mover.position, frameTime);
        }

        private Vector3 GetPidDiff(Vector3 target, Vector3 current, float frameTime)
        {
            float xDiff = _xPidController.GetPidDiff(target.x, current.x, frameTime);
            float yDiff = _yPidController.GetPidDiff(target.y, current.y, frameTime);
            return new Vector3(xDiff, yDiff, 0f);
        }

        public void ResetPid()
        {
            _xPidController.ResetPid();
            _yPidController.ResetPid();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}