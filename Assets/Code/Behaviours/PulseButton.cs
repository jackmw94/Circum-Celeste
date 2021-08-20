using UnityEngine;
using UnityEngine.UI;

namespace Code.Behaviours
{
    public class PulseButton : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [Space(15)] 
        [SerializeField] private float _frequency;
        [SerializeField] private Color _pulsedColour;

        private bool _pulsing = false;
        private Color _defaultColor;
        private float _startPulsingTime = 0f;

        private void Awake()
        {
            _button.onClick.AddListener(ButtonStopPulsing);
            _defaultColor = _image.color;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ButtonStopPulsing);
        }

        public void StartStopPulse(bool pulsing)
        {
            if (!_pulsing)
            {
                // if we're potentially starting pulsing
                _startPulsingTime = Time.time;
            }
            else if (!pulsing)
            {
                // if we're turning pulsing off
                _image.color = _defaultColor;
            }

            _pulsing = pulsing;
        }

        private void ButtonStopPulsing() => StartStopPulse(false);

        private void Update()
        {
            if (!_pulsing)
            {
                return;
            }

            float pulseTime = Time.time - _startPulsingTime;
            float sinVal = Mathf.Sin(pulseTime * _frequency - Mathf.PI / 2f);
            float lerpVal = sinVal / 2f + 0.5f;
            Color colour = Color.Lerp(_defaultColor, _pulsedColour, lerpVal);
            _image.color = colour;
        }
    }
}