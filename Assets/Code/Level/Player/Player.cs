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
        [SerializeField] private MovementRecorder _movementRecorder;

        public bool IsRecording { get; private set; } = false;
        public bool IsMoving => _mover.IsMoving;
        public bool IsDead => _health.IsDead;
        public bool NoDamageTaken => _health.NoDamageTaken;

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

            if (IsRecording)
            {
                _movementRecorder.StartRecording();
            }
        }

        public override void LevelFinished()
        {
            base.LevelFinished();
            
            TurnInputBehavioursOffOn(false);
            _health.IsInvulnerable = true;
            gameObject.SetActive(false);
            
            _movementRecorder.StopRecording();
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

        public LevelRecordingData GetLevelRecording()
        {
            if (!IsRecording)
            {
                return null;
            }
            
            return new LevelRecordingData
            {
                Positions = _movementRecorder.Positions,
                LevelTime = _movementRecorder.Duration
            };

        }
    }
}