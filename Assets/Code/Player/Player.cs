using System.Collections;
using Code.VFX;
using UnityEngine;

namespace Code.Player
{
    public class Player : MonoBehaviour
    {
        private const float InvulnerableDurationAtLevelStart = 1.5f;
        
        [SerializeField] private Transform _orbiter;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private PlayerMover _mover;
        [SerializeField] private SlingController _sling;

        private void Awake()
        {
            _health.SetOnDeathCallback(PlayerDied);
            _health.IsInvulnerable = true;
            
            TurnInputBehavioursOffOn(false);
        }

        public void Initialise(int maxHealth, InputProvider inputProvider)
        {
            _health.SetMaximumHealth(maxHealth);
            _health.ResetHealth();
            
            _input.Initialise(inputProvider);
        }

        public void StartLevel()
        {
            TurnInputBehavioursOffOn(true);
            StartCoroutine(SetVulnerableAfterDelay());
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
            _sling.SlingEnabled = inputIsOn;
        }
    }
}