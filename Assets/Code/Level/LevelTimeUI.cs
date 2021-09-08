using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelTimeUI : MonoBehaviour
    {
        [Flags]
        private enum OptionalUIVisibilitySetting
        {
            None = 0,
            UserSetting = 1 << 0,
            Gameplay = 1 << 1,
            Visible = UserSetting | Gameplay
        }
        
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showHideDuration = 0.25f;

        private OptionalUIVisibilitySetting _optionalUIVisibility = OptionalUIVisibilitySetting.None;
        private float _time = 0f;
        private bool _isRunning = false;

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

        public void GameplayShowHideTime(bool show)
        {
            UpdateVisibility(OptionalUIVisibilitySetting.Gameplay, show);
        }

        public void SettingsShowHideTime(bool show)
        {
            UpdateVisibility(OptionalUIVisibilitySetting.UserSetting, show);
        }

        private void UpdateVisibility(OptionalUIVisibilitySetting setting, bool showing)
        {
            if (showing)
            {
                _optionalUIVisibility |= setting;
            }
            else
            {
                _optionalUIVisibility &= ~setting;
            }
            
            ShowHideTimeInternal(_optionalUIVisibility == OptionalUIVisibilitySetting.Visible);
        }
        
        private void ShowHideTimeInternal(bool show)
        {
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
            UpdateLabel();
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