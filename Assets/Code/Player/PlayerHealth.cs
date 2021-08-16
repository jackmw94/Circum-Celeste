using UnityEngine;

namespace Code.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        private int _orbiterLayer;
        private int _playerHealth = 5;

        private void Awake()
        {
            _orbiterLayer = LayerMask.NameToLayer("Orbiter");
        }

        private void OnGUI()
        {
            Rect rect = new Rect(10, 10, 200, 50);
            GUI.Box(rect, _playerHealth.ToString());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _orbiterLayer)
            {
                Feedbacks.Instance.SetTriggerOrbiterHitsPlayerFeedbackActive(true);
                _playerHealth--;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == _orbiterLayer)
            {
                Feedbacks.Instance.SetTriggerOrbiterHitsPlayerFeedbackActive(false);
            }
        }
    }

    public class Player : MonoBehaviour
    {
        
    }
}