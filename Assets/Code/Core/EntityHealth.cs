using System;
using Code.Debugging;
using UnityEngine;

namespace Code.Core
{
    public abstract class EntityHealth : MonoBehaviour
    {
        private int _maximumHealth = 5;
            
        private int _currentHealth = 5;
    
        private Action _onDeath = null;
            
        public bool IsInvulnerable { get; set; }
    
        public bool IsDead => _currentHealth <= 0;
        public bool NoDamageTaken => _currentHealth == _maximumHealth;
        public float HealthFraction => _currentHealth / (float)_maximumHealth;
            
        public void SetMaximumHealth(int maximumHealth)
        {
            _maximumHealth = maximumHealth;
        }
    
        public void SetOnDeathCallback(Action onDeath)
        {
            _onDeath = onDeath;
        }
    
        public void ResetHealth()
        {
            _currentHealth = _maximumHealth;
            OnHealthReset();
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (IsDead || IsInvulnerable)
            {
                return;
            }
                
            if (DoesObjectDamageUs(other.gameObject))
            {
                HitTaken();
            }
        }
    
        private void HitTaken()
        {
            _currentHealth--;

            if (IsDead)
            {
                _onDeath?.Invoke();
            }

            OnHitTaken();
            CircumDebug.Log($"{gameObject} taken damage. Health = {_currentHealth}");
        }

        protected abstract bool DoesObjectDamageUs(GameObject gameObj);

        protected virtual void OnHitTaken(){}
        protected virtual void OnHealthReset(){}
    }
}