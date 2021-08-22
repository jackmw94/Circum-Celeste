using UnityEngine;

namespace Code.Juice
{
    /// <summary>
    /// Screen shake assumes object doesn't move - it sets the origin position on awake then keeps it there when updating
    /// Must work on unscaled time to not get affected by the time control feedback system
    /// </summary>
    public class ScreenShake : MonoBehaviour
    {
        [SerializeField] private float _frequency = 1f;
        [SerializeField] private float _addShakeMagnitude = 0.01f;
        [SerializeField] private float _maximumShakeMagnitude = 0.03f;
        [SerializeField] private float _calmDownTime = 0.3f;
        [Space(30)]
        [SerializeField] private float _debugAddShakeFactor = 1f;
        [SerializeField] private bool _debugAddShake;

        private Vector3 _defaultPosition = Vector3.zero;
        private float _shakeAmount = 0f;

        private void Awake()
        {
            _defaultPosition = transform.localPosition;
        }

        private void Update()
        {
            if (_debugAddShake)
            {
                _debugAddShake = false;
                AddShake(_debugAddShakeFactor);
            }
            
            Shake();

            float shakeReduction = Time.unscaledDeltaTime * _addShakeMagnitude / _calmDownTime;
            _shakeAmount -= shakeReduction;
            _shakeAmount = Mathf.Max(_shakeAmount, 0f);
        }

        private void Shake()
        {
            float perlinX = Mathf.PerlinNoise(Time.unscaledTime * _frequency, 0f) * 2 - 1f;
            float perlinNoiseY = Mathf.PerlinNoise(Time.unscaledTime * _frequency, 100f) * 2f - 1f;
            
            float shakeX = perlinX * _shakeAmount;
            float shakeY = perlinNoiseY * _shakeAmount;

            transform.localPosition = _defaultPosition + new Vector3(shakeX, shakeY, 0f);
        }

        public void AddShake(float factor)
        {
            _shakeAmount += factor * _addShakeMagnitude;
            _shakeAmount = Mathf.Clamp(_shakeAmount, 0f, _maximumShakeMagnitude);
        }
    }
}
