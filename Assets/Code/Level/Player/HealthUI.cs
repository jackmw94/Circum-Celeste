using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Level.Player
{ 
    public class HealthUI : SingletonMonoBehaviour<HealthUI>
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