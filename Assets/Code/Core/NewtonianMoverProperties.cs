using UnityEngine;

namespace Code.Core
{
    [CreateAssetMenu(menuName = "Create NewtonianMoverProperties", fileName = "NewtonianMoverProperties", order = 0)]
    public class NewtonianMoverProperties : ScriptableObject
    {
        [SerializeField] private float _forceScalar = 1f;
        [SerializeField] private float _inertia = 0.01f;
        [SerializeField] private AnimationCurve _radiusToGravityForce;

        public float ForceScalar => _forceScalar;
        public float Inertia => _inertia;
        public AnimationCurve RadiusToGravityForce => _radiusToGravityForce;
    }
}