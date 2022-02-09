using UnityEngine;

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create BeamHazardSettings", fileName = "BeamHazardSettings", order = 0)]
    public class BeamHazardSettings : ScriptableObject
    {
        [SerializeField] private bool _triggerOnBothSides = true;
        [SerializeField] private Vector2 _beamBetweenAngles = new Vector2(90f, 180f);
        [SerializeField] private float _warmUpAngle = 45f;
        [SerializeField] private float _rotationSpeed = 20f;
        [SerializeField] private float _startingAngle;

        public bool TriggerOnBothSides => _triggerOnBothSides;
        public Vector2 BeamBetweenAngles => _beamBetweenAngles;
        public float WarmUpAngle => _warmUpAngle;
        public float RotationSpeed => _rotationSpeed;
        public float StartingAngle => _startingAngle;
    }
}