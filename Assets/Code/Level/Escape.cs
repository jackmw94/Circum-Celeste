using System;
using Code.Core;
using Code.VFX;
using UnityEngine;

namespace Code.Level
{
    public class Escape : MonoBehaviour
    {
        private Action _onEscapeEntered = null;

        public void SetEscapeCallback(Action onEscapedCallback)
        {
            _onEscapeEntered = onEscapedCallback;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            HandleCollision(other.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other.gameObject);
        }

        private void HandleCollision(GameObject other)
        {
            if (other.IsPlayer())
            {
                EscapeEntered();
            }
        }

        private void EscapeEntered()
        {
            VfxManager.Instance.SpawnVfx(VfxType.PlayerEscaped, transform.position);
            _onEscapeEntered?.Invoke();
        }
    }
}