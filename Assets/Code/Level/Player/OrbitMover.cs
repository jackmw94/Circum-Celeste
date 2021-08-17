using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    [System.Serializable]
    public class OrbitMover : GravitationalMover
    {
        private float _slingIncrease;
        
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