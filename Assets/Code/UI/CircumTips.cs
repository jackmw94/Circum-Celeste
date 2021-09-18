using System;
using System.Collections;
using Code.Behaviours;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.UI
{
    public class CircumTips : MonoBehaviour
    {
        [SerializeField] private AnimateImageShaderProperty _showTipsBackground;
        [SerializeField] private AnimateImageShaderProperty _hideTipsBackground;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _backgroundImage;
        [Space(15)]
        [SerializeField] private Button _backButton;
        
        private Coroutine _showHideCoroutine = null;

        private void Awake()
        {
            _backButton.onClick.AddListener(BackButtonListener);
            
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0f;
            _backgroundImage.raycastTarget = false;
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(BackButtonListener);
        }

        public void ShowHideTipsScreen(bool show)
        {
            if (_showHideCoroutine != null)
            {
                StopCoroutine(_showHideCoroutine);
            }
            _showHideCoroutine = StartCoroutine(show ? ShowInternal() : HideInternal());
        }

        private IEnumerator ShowInternal()
        {
            gameObject.SetActiveSafe(true);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _backgroundImage.raycastTarget = true;
            yield return RunAnimateImageShaderProperty(_showTipsBackground);
            yield return new WaitForSeconds(0.25f);
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, 1f, 0.5f, f => _canvasGroup.alpha = f);
        }

        private IEnumerator HideInternal()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _backgroundImage.raycastTarget = false;
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, 0f, 0.5f, f => _canvasGroup.alpha = f);
            yield return RunAnimateImageShaderProperty(_hideTipsBackground);
            gameObject.SetActiveSafe(false);
        }

        private void BackButtonListener()
        {
            ShowHideTipsScreen(false);
        }

        private static IEnumerator RunAnimateImageShaderProperty(AnimateImageShaderProperty animateImageShaderProperty)
        {
            bool animationCompleted = false;
            animateImageShaderProperty.TriggerAnimation(() =>
            {
                animationCompleted = true;
            });

            yield return new WaitUntil(() => animationCompleted);
        }
    }
}