using System;
using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class OrbitMover : GravitationalMover
    {
        private float _slingIntegralOffset;
        private float _slingProportionalOffset;

        private void Awake()
        {
            float p = RemoteConfigHelper.OrbiterP;
            float i = RemoteConfigHelper.OrbiterI;
            float d = RemoteConfigHelper.OrbiterD;
            _xPidController.SetPIDValues(p, i, d);
            _yPidController.SetPIDValues(p, i, d);
        }

        protected override Vector3 GetMovement(Vector3 targetPosition, Transform mover, float frameTime)
        {
            _xPidController.SetPidOffsets(_slingIntegralOffset, _slingProportionalOffset);
            _yPidController.SetPidOffsets(_slingIntegralOffset, _slingProportionalOffset);
            return base.GetMovement(targetPosition, mover, frameTime);
        }
        
        public void SetSlingOffsets(float integralOffset, float proportionalOffset)
        {
            _slingIntegralOffset = integralOffset;
            _slingProportionalOffset = proportionalOffset;
        }
    }
}