using System.Collections;
using UnityCommonFeatures;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class ChallengeInfoToggle : ButtonBehaviour
    {
        [SerializeField] private Image _overlay;
        [SerializeField] private Image _infoMask;
        [SerializeField] private Vector2 _fillAmountMinMax;
        [SerializeField] private float _transitionDuration;

        private bool _isOn = false;
        private Coroutine _turnOnOffCoroutine;

        protected override void OnButtonClicked()
        {
            _isOn = !_isOn;
            this.RestartCoroutine(ref _turnOnOffCoroutine, TurnChallengeInfoOnOff(_isOn));
        }

        private IEnumerator TurnChallengeInfoOnOff(bool on)
        {
            float targetValue = on ? _fillAmountMinMax.y : _fillAmountMinMax.x;
            float fillDifference = _fillAmountMinMax.y - _fillAmountMinMax.x;
            float duration = _transitionDuration / fillDifference;
            
            if (on) _overlay.raycastTarget = true;
            
            yield return Utilities.LerpOverTime(_infoMask.fillAmount, targetValue, duration, f =>
            {
                _infoMask.fillAmount = f;
            });
            
            if (!on) _overlay.raycastTarget = false;
        }
    }
}