using System;
using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class OrbitMover : GravitationalMover
    {
        private float _slingIncrease;

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
            _xPidController.SetMaxIntegralOffset(_slingIncrease);
            _yPidController.SetMaxIntegralOffset(_slingIncrease);
            return base.GetMovement(targetPosition, mover, frameTime);
        }
        
        public void SetSlingIncrease(float slingIncrease)
        {
            _slingIncrease = slingIncrease;
        }
    }
}