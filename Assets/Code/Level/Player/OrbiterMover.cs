using System;
using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class OrbiterMover : GravitationalMover
    {
        private float _slingIntegralOffset;
        private float _slingProportionalOffset;

        protected override float MovementSizeScaler => 1f;

        protected override Vector3 GetMovement(Vector3 targetPosition, Vector3 currentPosition, float frameTime)
        {
            _xPidController.SetPidOffsets(_slingIntegralOffset, _slingProportionalOffset);
            _yPidController.SetPidOffsets(_slingIntegralOffset, _slingProportionalOffset);
            return base.GetMovement(targetPosition, currentPosition, frameTime);
        }
        
        public void SetSlingOffsets(float integralOffset, float proportionalOffset)
        {
            _slingIntegralOffset = integralOffset;
            _slingProportionalOffset = proportionalOffset;
        }

        public void SetPidValues(Vector3 pidValues)
        {
            _xPidController.SetPIDValues(pidValues.x, pidValues.y, pidValues.z);
            _yPidController.SetPIDValues(pidValues.x, pidValues.y, pidValues.z);
        }
    }
}