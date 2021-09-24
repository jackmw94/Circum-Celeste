using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.EasingFunctions;

namespace Code.Flow
{
    public class AnimateSettingsSection : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _settingsButtonCanvasGroup;
        [SerializeField] private Image _settingsButtonImage;
        [SerializeField] private Sprite _settingsButtonSprite;
        [SerializeField] private Sprite _backButtonSprite;
        [Space(15)]
        [SerializeField] private Transform _topSettingsRoot;
        [SerializeField] private float _topSettingsHiddenLocalYPosition;
        [Space(15)]
        [SerializeField] private Transform _sectionRoot;
        [SerializeField] private float _settingsSectionHiddenLocalYPosition;
        [Space(15)]
        [SerializeField] private EasingFunctions.EasingType _easingType;
        [SerializeField] private float _duration = 0.25f;

        private bool _topLevelCurrentlyShowing = true;
        private Coroutine _animateSettingsSectionCoroutine = null;
        private float _currentAnimateValue = 0f;

        public bool TopLevelCurrentlyShowing => _topLevelCurrentlyShowing;

        public void ShowTopSettings()
        {
            AnimateSettingsInternal(true);
        }

        public void ShowSettingsSection()
        {
            AnimateSettingsInternal(false);
        }

        private void AnimateSettingsInternal(bool showingTopSettings)
        {
            if (_animateSettingsSectionCoroutine != null)
            {
                StopCoroutine(_animateSettingsSectionCoroutine);
            }
            _animateSettingsSectionCoroutine = StartCoroutine(ShowSettings(showingTopSettings));
        }

        private IEnumerator ShowSettings(bool showingTopSettings)
        {
            float targetValue = showingTopSettings ? 0f : 1f;
            _settingsButtonCanvasGroup.interactable = false;
            yield return Utilities.LerpOverTime(_currentAnimateValue, targetValue, _duration, f =>
            {
                _currentAnimateValue = EasingFunctions.ConvertLinearToEased(_easingType, EasingFunctions.EasingDirection.InAndOut, f);
                UpdateAnimationValue();
            });
            _settingsButtonCanvasGroup.interactable = true;
            _topLevelCurrentlyShowing = showingTopSettings;
        }

        /// <summary>
        /// A value of 0 is the top level settings fully shown and a value of 1 is the settings section being fully shown
        /// </summary>
        private void UpdateAnimationValue()
        {
            Vector3 topSettingsPosition = _topSettingsRoot.localPosition;
            Vector3 settingsSectionPosition = _sectionRoot.localPosition;

            topSettingsPosition.y = Mathf.Lerp(0f, _topSettingsHiddenLocalYPosition, _currentAnimateValue);
            settingsSectionPosition.y = Mathf.Lerp(_settingsSectionHiddenLocalYPosition, 0f, _currentAnimateValue);

            _topSettingsRoot.localPosition = topSettingsPosition;
            _sectionRoot.localPosition = settingsSectionPosition;

            _settingsButtonCanvasGroup.alpha = 1f - EasingFunctions.Triangle(_currentAnimateValue);
            _settingsButtonImage.sprite = _currentAnimateValue < 0.5f ? _settingsButtonSprite : _backButtonSprite;
        }
    }
}