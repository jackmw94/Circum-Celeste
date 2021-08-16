using System;
using Code.Core;
using UnityEngine;

namespace Code.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        private int _maximumHealth = 5;
        
        private int _playerHealth = 5;

        private Action _onPlayerDeath = null;
        
        public bool IsInvulnerable { get; set; }

        public bool IsDead => _playerHealth <= 0;
        
        public void SetMaximumHealth(int maximumHealth)
        {
            _maximumHealth = maximumHealth;
        }

        public void SetOnDeathCallback(Action onPlayerDeath)
        {
            _onPlayerDeath = onPlayerDeath;
        }

        public void ResetHealth()
        {
            _playerHealth = _maximumHealth;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsDead || IsInvulnerable)
            {
                return;
            }
            
            if (ObjectDamagesPlayer(other.gameObject))
            {
                Feedbacks.Instance.TriggerHealthLostFeedback();
                HitTaken();
            }
        }

        private void HitTaken()
        {
            Debug.Log("Player taken damage");
            _playerHealth--;

            if (IsDead)
            {
                _onPlayerDeath?.Invoke();
            }
        }

        private static bool ObjectDamagesPlayer(GameObject gameObj)
        {
            return gameObj.IsOrbiter() || gameObj.IsEnemy();
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                IsInvulnerable = !IsInvulnerable;
                Debug.Log($"Debug setting player to {(IsInvulnerable ? "invulnerable" : "not invulnerable")}");
            }
        }
#endif
    }
}