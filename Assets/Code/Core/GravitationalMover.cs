using Code.Debugging;
using UnityEngine;
using UnityExtras.Core;

namespace Code.Core
{
    public class GravitationalMover : OrbiterMover
    {
        [SerializeField] protected PidController _xPidController;
        [SerializeField] protected PidController _yPidController;
        
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
            Vector3 movement = GetMovement(_target.position, transform.position, Time.fixedDeltaTime);
            transform.Translate(movement, Space.World);
        }

        protected virtual Vector3 GetMovement(Vector3 targetPosition, Vector3 currentPosition, float frameTime)
        {
            // by increasing the scale of the positions then we can mimic the same movement of the orbiter at different scales
            targetPosition /= MovementSizeScaler;
            currentPosition /= MovementSizeScaler;
            
            return GetPidDiff(targetPosition, currentPosition, frameTime);
        }

        private Vector3 GetPidDiff(Vector3 target, Vector3 current, float frameTime)
        {
            float xDiff = _xPidController.GetPidDiff(target.x, current.x, frameTime);
            float yDiff = _yPidController.GetPidDiff(target.y, current.y, frameTime);
            return new Vector3(xDiff, yDiff, 0f);
        }

        public override void SetupGridSize(int gridSize)
        {
            base.SetupGridSize(gridSize);

            Vector3 pidValues = RemoteConfigHelper.GetOrbiterPidValuesFromGridSize(gridSize);
            SetPidValues(pidValues);
            
            CircumDebug.Log($"Pid values for orbiter : {pidValues.ToPreciseString()}");
        }

        public void ResetPid()
        {
            _xPidController.ResetPid();
            _yPidController.ResetPid();
        }
        
        public void SetPidValues(Vector3 pidValues)
        {
            _xPidController.SetPIDValues(pidValues.x, pidValues.y, pidValues.z);
            _yPidController.SetPIDValues(pidValues.x, pidValues.y, pidValues.z);
        }
    }
}