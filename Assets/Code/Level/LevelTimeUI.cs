using System.Collections;
using TMPro;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelTimeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showHideDuration = 0.25f;

        private float _time = 0f;
        private bool _isRunning = false;
        private bool _isShowing = false;

        private Coroutine _showHideTimerCoroutine = null;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (!_isRunning)
            {
                return;
            }
            
            UpdateLabel();
            _time += Time.deltaTime;
        }

        private void UpdateLabel()
        {
            _text.text = _time.ToString("F2");
        }

        public void ShowHideTimer(bool show)
        {
            if (show == _isShowing)
            {
                return;
            }

            _isShowing = show;
            
            if (_showHideTimerCoroutine != null)
            {
                StopCoroutine(_showHideTimerCoroutine);
            }
            _showHideTimerCoroutine = StartCoroutine(ShowHideTimerCoroutine(show));
        }

        public void ManuallySetTimer(float time)
        {
            _time = time;
            UpdateLabel();
        }

        public void ResetTimer()
        {
            _time = 0f;
        }

        public void StartStopTimer(bool start)
        {
            if (start)
            {
                ResetTimer();
            }

            _isRunning = start;
        }

        private IEnumerator ShowHideTimerCoroutine(bool show)
        {
            float targetValue = show ? 1f : 0f;
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, targetValue, _showHideDuration, f =>
            {
                _canvasGroup.alpha = f;
            });
        }
    }
}