using UnityEngine;
using UnityExtras.Core;

namespace Code.Behaviours
{
    public class PulseColour : IntroductionBehaviour
    {
        [SerializeField] private Colourable _colourable;
        [Space(15)] 
        [SerializeField] private float _frequency;
        [SerializeField, ColorUsage(true, true)] private Color _pulsedColour;

        private Color _defaultColor;
        private float _startPulsingTime = 0f;

        private void Awake()
        {
            _defaultColor = _colourable.GetColour();
        }

        private void OnEnable()
        {
            _startPulsingTime = Time.time;
        }

        private void OnDisable()
        {
            _colourable.SetColour(_defaultColor);
        }

        private void Update()
        {
            float pulseValue = Utilities.GetSinePulse(_startPulsingTime, _frequency);
            Color colour = Color.Lerp(_defaultColor, _pulsedColour, pulseValue);
            _colourable.SetColour(colour);
        }
    }
}