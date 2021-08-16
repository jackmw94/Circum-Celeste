using Code.Core;
using UnityEngine;

namespace Code.Player
{
    [System.Serializable]
    public class OrbitMover : MonoBehaviour
    {
        [SerializeField] private PidController _xPidController;
        [SerializeField] private PidController _yPidController;
        [Space(15)]
        [SerializeField] private Transform _target;

        private float _slingIncrease;

        private void Update()
        {
            UpdatePosition(_target.position, transform, Time.deltaTime);
        }

        public Vector3 GetPidDiff(Vector3 target, Vector3 current, float frameTime)
        {
            float xDiff = _xPidController.GetPidDiff(target.x, current.x, frameTime);
            float yDiff = _yPidController.GetPidDiff(target.y, current.y, frameTime);
            return new Vector3(xDiff, yDiff, 0f);
        }

        public void UpdatePosition(Vector3 targetPosition, Transform mover, float frameTime)
        {
            _xPidController.SetMaxIntegralOffset(_slingIncrease);
            _yPidController.SetMaxIntegralOffset(_slingIncrease);
        
            mover.Translate(GetPidDiff(targetPosition, mover.position, frameTime), Space.World);
        }

        public void ResetPid()
        {
            _xPidController.ResetPid();
            _yPidController.ResetPid();
        }

        public void SetSlingIncrease(float slingIncrease)
        {
            _slingIncrease = slingIncrease;
        }
    }
}