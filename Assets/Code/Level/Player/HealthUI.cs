using UnityEngine;

namespace Code.Level.Player
{ 
    public class HealthUI : MonoBehaviour
    {
        public void UpdateHealthBar(int playerIndex, float fraction)
        {
            Transform t = transform;
            Vector3 localScale = t.localScale;
            localScale.x = fraction;
            t.localScale = localScale;
        }
    }
}