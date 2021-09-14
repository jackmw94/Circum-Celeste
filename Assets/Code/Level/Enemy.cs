using Code.Core;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class Enemy : LevelElement
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _levelFinishedAnimationTrigger = "Dissipate";
        [Space(15)]
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private EnemyHealth _enemyHealth;
        [SerializeField] private GravitationalMover _gravitationalMover;

        public bool IsDead => _enemyHealth.IsDead;

        private void Awake()
        {
            if (_gravitationalMover) _gravitationalMover.enabled = false;
            _collider.radius = RemoteConfigHelper.EnemyColliderRadius;
        }
        
        public override void LevelStarted()
        {
            base.LevelStarted();
            
            Player.Player[] allPlayers = FindObjectsOfType<Player.Player>();
            Transform targetPlayer = allPlayers.GetNext(0, Random.Range(0,10)).transform;

            if (_gravitationalMover)
            {
                // not present when replaying
                _gravitationalMover.SetTarget(targetPlayer);
                _gravitationalMover.enabled = true;
            }

            _enemyHealth.ResetHealth();
        }

        public override void LevelFinished()
        {
            base.LevelFinished();
            
            _animator.SetTrigger(_levelFinishedAnimationTrigger);

            //if (_gravitationalMover) _gravitationalMover.enabled = false;
        }
    }
}