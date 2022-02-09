using Code.Core;
using UnityEngine;

namespace Code.Level
{
    public class BeamHazard : YeetableGravitationalMover
    {
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private BeamHazardStateMachine _beamHazardStateMachine;

        private void Awake()
        {
            _collider.radius = RemoteConfigHelper.HazardColliderSize;
        }
        
        public override void LevelStarted()
        {
            base.LevelStarted();
            _beamHazardStateMachine.LevelStarted = true;
        }

        public override void LevelFinished()
        {
            base.LevelFinished();
            _beamHazardStateMachine.LevelStarted = false;
        }
    }
}