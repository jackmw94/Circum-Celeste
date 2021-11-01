using System;
using System.Collections;
using System.Collections.Generic;
using Code.Debugging;
using UnityEngine;

namespace Code.Core
{
    public abstract class EntityHealth : MonoBehaviour
    {
        private const float DefaultOnStayDamageDelay = 2.25f;
        
        protected float _onStayDamageDelay = DefaultOnStayDamageDelay;
        
        private readonly Dictionary<GameObject, Coroutine> _onStayDamageAppliers = new Dictionary<GameObject, Coroutine>();
        private int _maximumHealth = 5;
        private int _currentHealth = 5;
        private Action _onDeath = null;

        public bool GameStateInvulnerable { private get; set; }
        public bool LevelInvulnerable { private get; set; }

        public bool IsInvulnerable => GameStateInvulnerable || LevelInvulnerable;
    
        public bool IsDead => _currentHealth <= 0;
        public bool NoDamageTaken => _currentHealth == _maximumHealth;
        public int CurrentHealth => _currentHealth;
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
            HandleObjectCollision(other.gameObject, true);
        }

        private void OnTriggerStay(Collider other)
        {
            HandleObjectCollision(other.gameObject, false);
        }

        private void HandleObjectCollision(GameObject other, bool isEntering)
        {
            if (!DoesObjectDamageUs(other))
            {
                return;
            }
            
            if (isEntering)
            {
                HitTaken();
            }
            
            // checking invulnerable so we don't start this before player moves
            if (!IsInvulnerable && !_onStayDamageAppliers.ContainsKey(other))
            {
                _onStayDamageAppliers.Add(other, StartCoroutine(HandleOnStayDamage()));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_onStayDamageAppliers.TryGetValue(other.gameObject, out Coroutine onStayDamageApplierCoroutine))
            {
                StopCoroutine(onStayDamageApplierCoroutine);
                _onStayDamageAppliers.Remove(other.gameObject);
            }
        }

        private IEnumerator HandleOnStayDamage()
        {
            yield return new WaitForSeconds(_onStayDamageDelay);
            
            // if we shuffle around without orbiting then it'll kill ya
            // this coroutine is stopped if object exits
            HitTaken(_maximumHealth);
        }
        
        public void HitTaken(int hitDamage = 1)
        {
            if (IsDead || IsInvulnerable)
            {
                return;
            }
            
            _currentHealth = Mathf.Max(_currentHealth - hitDamage, 0);
            
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