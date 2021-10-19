using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.UI
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _loadingOverlay;
        [SerializeField] private float _loadingHideDuration = 0.3f;
        
        private Coroutine _showHideLoadingCoroutine = null;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_loadingOverlay)
            {
                _loadingOverlay = GetComponent<CanvasGroup>();
            }
        }

        public void ShowHideLoading(bool show, bool instant)
        {
            if (_showHideLoadingCoroutine != null)
            {
                StopCoroutine(_showHideLoadingCoroutine);
            }
            _showHideLoadingCoroutine = StartCoroutine(ShowHideCoroutine(show, instant));
        }
        
        private IEnumerator ShowHideCoroutine(bool show, bool instant)
        {
            _loadingOverlay.blocksRaycasts = true;
            float targetValue = show ? 1f : 0f;
            float zeroToOneDuration = instant ? 0f : _loadingHideDuration;
            yield return Utilities.LerpOverTime(_loadingOverlay.alpha, targetValue, zeroToOneDuration, f =>
            {
                _loadingOverlay.alpha = f;
            });
            _loadingOverlay.blocksRaycasts = show;
        }
    }
}