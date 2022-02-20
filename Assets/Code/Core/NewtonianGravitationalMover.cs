using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Core
{
    public class NewtonianGravitationalMover : OrbiterMover
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private NewtonianMoverProperties _moverProperties;
        
        private void FixedUpdate()
        {
            if (!_target)
            {
                return;
            }
            
#if UNITY_EDITOR
            if (Input.GetKeyDown(EditorKeyCodeBindings.ResetOrbiter))
            {
                transform.localPosition = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
            }
#endif
            
            FixedUpdateInternal();
        }

        protected virtual void FixedUpdateInternal()
        {
            Vector3 targetOffset = _target.position - transform.position;
            Vector3 direction = targetOffset.normalized;
            float gravityForce = _moverProperties.RadiusToGravityForce.Evaluate(targetOffset.magnitude) * _moverProperties.ForceScalar;
            Vector3 forceVector = direction * (Time.fixedTime * gravityForce);

            _rigidbody.velocity *= (1f - _moverProperties.Inertia);
            
            _rigidbody.AddForce(forceVector, ForceMode.Acceleration);
            
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