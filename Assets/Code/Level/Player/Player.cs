using System.Collections;
using Code.Core;
using Code.VFX;
using UnityEngine;

namespace Code.Level.Player
{
    public class Player : LevelElement
    {
        private const float InvulnerableDurationAtLevelStart = 1.5f;

        [SerializeField] private GameObject _orbiter;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private PlayerMover _mover;
        [SerializeField] private SlingController _sling;

        public bool IsMoving => _mover.IsMoving;
        public bool IsDead => _health.IsDead;
        public bool NoDamageTaken => _health.NoDamageTaken;

        private void Awake()
        {
            _health.SetOnDeathCallback(PlayerDied);
            _health.IsInvulnerable = true;

            TurnInputBehavioursOffOn(false);
        }

        public void Initialise(int maxHealth, InputProvider inputProvider, bool orbiterEnabled, bool powerEnabled)
        {
            _health.SetMaximumHealth(maxHealth);
            _health.ResetHealth();

            _sling.SlingEnabled = powerEnabled;
            _orbiter.SetActive(orbiterEnabled);

            _input.Initialise(inputProvider);
        }

        public void LevelReady()
        {
            TurnInputBehavioursOffOn(true);
        }

        public override void LevelStarted()
        {
            base.LevelStarted();
            StartCoroutine(SetVulnerableAfterDelay());
        }

        public override void LevelFinished()
        {
            base.LevelFinished();
            
            TurnInputBehavioursOffOn(false);
            _health.IsInvulnerable = true;
            gameObject.SetActive(false);
        }

        private void PlayerDied()
        {
            TurnInputBehavioursOffOn(false);
            VfxManager.Instance.SpawnVfx(VfxType.PlayerDied, transform.position);
        }

        private IEnumerator SetVulnerableAfterDelay()
        {
            yield return new WaitForSeconds(InvulnerableDurationAtLevelStart);
            _health.IsInvulnerable = false;
        }

        private void TurnInputBehavioursOffOn(bool inputIsOn)
        {
            _mover.MovementEnabled = inputIsOn;
        }
    }
}