using System.Collections;
using UnityEngine;
using UnityExtras.Code.Optional.EasingFunctions;
using UnityExtras.Core;

namespace Code.Behaviours
{
    public class IntroductionCanvasGroup : IntroductionBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showHideDuration = 0.25f;
        [SerializeField] private EasingFunctions.EasingType _easingType;
        
        private Coroutine _showHideCanvasCoroutine = null;
        
        public override void SetEnabled(bool isEnabled)
        {
            if (_showHideCanvasCoroutine != null)
            {
                StopCoroutine(_showHideCanvasCoroutine);
            }
            _showHideCanvasCoroutine = StartCoroutine(ShowHideCanvasGroupCoroutine(isEnabled));
        }

        private IEnumerator ShowHideCanvasGroupCoroutine(bool isShowing)
        {
            float targetValue = isShowing ? 1f : 0f;
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, targetValue, _showHideDuration, f =>
            {
                float easedF = EasingFunctions.ConvertLinearToEased(_easingType, EasingFunctions.EasingDirection.InAndOut, f);
                _canvasGroup.alpha = easedF;
            });
        }
    }
}