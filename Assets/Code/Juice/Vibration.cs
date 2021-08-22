using UnityEngine;

namespace Code.Juice
{
    /// <summary>
    /// Must work on unscaled time to not get affected by the time control feedback system
    /// </summary>
    public class Vibration : MonoBehaviour
    {
        private float _vibrationDuration;
        
        private void Update()
        {
            if (_vibrationDuration <= 0f)
            {
                return;
            }
            
            Handheld.Vibrate();
            _vibrationDuration -= Time.unscaledDeltaTime;
        }

        public void AddVibration(float duration)
        {
            _vibrationDuration = Mathf.Max(duration, _vibrationDuration);
        }
    }
}