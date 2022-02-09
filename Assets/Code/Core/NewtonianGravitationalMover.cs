using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Core
{
    public class NewtonianGravitationalMover : OrbiterMover
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _forceScalar = 0.1f;
        [SerializeField] private float _bounceDistance = 0.1f;
        [SerializeField] private float _bounceForce = 0.1f;
        [SerializeField] private float _velocityReduction = 0.01f;
        [SerializeField] private Vector2 _clampTargetRadius = new Vector2(0.1f, 3f);
        
        private void FixedUpdate()
        {
            if (!_target)
            {
                return;
            }

            if (Input.GetKeyDown(EditorKeyCodeBindings.ResetOrbiter))
            {
                transform.localPosition = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
            }
            
            FixedUpdateInternal();
        }

        protected virtual void FixedUpdateInternal()
        {
            Vector3 targetOffset = _target.position - transform.position;
            Vector3 direction = targetOffset.normalized;
            float targetOffsetMagnitude = Mathf.Clamp(targetOffset.magnitude, _clampTargetRadius.x, _clampTargetRadius.y);
            Vector3 forceVector = direction * (Time.fixedTime * _forceScalar) / (targetOffsetMagnitude * targetOffsetMagnitude);
            _rigidbody.velocity = _rigidbody.velocity * _velocityReduction;
            _rigidbody.AddForce(forceVector, ForceMode.Acceleration);

            if (targetOffset.magnitude <= _bounceDistance)
            {
                if (Vector3.Dot(_rigidbody.velocity, direction) > 0)
                {
                    _rigidbody.velocity = Vector3.Reflect(_rigidbody.velocity, transform.position - _target.position);
                }
                _rigidbody.AddForce(_rigidbody.velocity.normalized * _bounceForce, ForceMode.Acceleration);
            }

            if (transform.position.x > 5f)
            {
                transform.position = transform.position.ModifyVectorElement(0, 5f);
                if (_rigidbody.velocity.x > 0) _rigidbody.velocity = Vector3.Reflect(_rigidbody.velocity, Vector3.left);
            }

            if (transform.position.x < -5f)
            {
                transform.position = transform.position.ModifyVectorElement(0, -5f);
                if (_rigidbody.velocity.x < 0) _rigidbody.velocity = Vector3.Reflect(_rigidbody.velocity, Vector3.right);
            }

            if (transform.position.y > 5f)
            {
                transform.position = transform.position.ModifyVectorElement(1, 5f);
                if (_rigidbody.velocity.y > 0) _rigidbody.velocity = Vector3.Reflect(_rigidbody.velocity, Vector3.down);
            }

            if (transform.position.y < -5f)
            {
                transform.position = transform.position.ModifyVectorElement(1, -5f);
                if (_rigidbody.velocity.y < 0) _rigidbody.velocity = Vector3.Reflect(_rigidbody.velocity, Vector3.up);
            }
        }
    }
}