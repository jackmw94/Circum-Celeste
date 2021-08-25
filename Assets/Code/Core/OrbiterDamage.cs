using Code.Level.Player;
using UnityEngine;

namespace Code.Core
{
    public class OrbiterDamage : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _playerHealth;
        
        private void OnTriggerEnter(Collider other)
        {
#if UNITY_EDITOR
            if (CheatsManager.Instance.PlayerHealthLossDisabled)
            {
                return;
            }
#endif
            
            if (other.gameObject.IsHazard())
            {
                _playerHealth.HitTaken();
            }
        }
    }
}