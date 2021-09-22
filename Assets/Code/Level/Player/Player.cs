using System.Collections;
using Code.Core;
using Code.Debugging;
using Code.VFX;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level.Player
{
    public class Player : LevelElement
    {
        private const float OrbiterWontDamageForDurationAtLevelStart = 1.5f;

        [SerializeField] private Orbiter _orbiter;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private PlayerMover _mover;
        [SerializeField] private SlingController _sling;

        public bool IsRecording { get; private set; } = false;
        public bool IsMoving => _mover.IsMoving;
        public bool IsDead => _health.IsDead;
        public bool NoDamageTaken => _health.NoDamageTaken;
        public Transform OrbiterTransform => _orbiter.transform;

        private void Awake()
        {
            _health.SetOnDeathCallback(PlayerDied);
            _health.IsInvulnerable = true;

            TurnInputBehavioursOffOn(false);
        }

        public void Initialise(int maxHealth, InputProvider inputProvider, bool orbiterEnabled, bool powerEnabled, bool isRecording = true)
        {
            IsRecording = isRecording;

            _health.SetMaximumHealth(maxHealth);
            _health.ResetHealth();

            _sling.SlingEnabled = powerEnabled;
            _orbiter.gameObject.SetActive(orbiterEnabled);

            _input.Initialise(inputProvider);
        }

        public override void LevelReady()
        {
            base.LevelReady();
            TurnInputBehavioursOffOn(true);
        }

        public override void LevelStarted()
        {
            base.LevelStarted();

            _health.IsInvulnerable = false;
            _health.OrbiterCanDamage = false;
            StartCoroutine(SetOrbiterDamageOnAfterDelay());
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

        private IEnumerator SetOrbiterDamageOnAfterDelay()
        {
            yield return new WaitForSeconds(OrbiterWontDamageForDurationAtLevelStart);
            _health.OrbiterCanDamage = true;
        }

        private void TurnInputBehavioursOffOn(bool inputIsOn)
        {
            _mover.MovementEnabled = inputIsOn;
        }

        public void SetOrbiterSpeedConfiguration(Vector3 pidValues)
        {
            CircumDebug.Log($"Pid values for orbiter : {pidValues.ToPreciseString()}");
            _orbiter.SetPidValues(pidValues);
        }
    }
}