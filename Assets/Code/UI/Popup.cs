using System;
using System.Collections;
using System.Collections.Generic;
using Code.Debugging;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.Singletons;

namespace Code.UI
{
    public class Popup : SingletonMonoBehaviour<Popup>
    {
        public enum LocalisedPopupType
        {
            CantRefreshConfig,
            SeeHowToPlay,
            CompletedGame
        }

        [Serializable]
        private struct LocalisedPopup
        {
            public LocalisedPopupType PopupType;
            [LeanTranslationName] public string LocalisationTerm;
        }
        
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _okayButton;
        [SerializeField] private TextMeshProUGUI _popupMessageText;
        [Space(15)]
        [SerializeField] private float _showHideDuration = 0.25f;
        [Space(15)]
        [SerializeField] private LocalisedPopup[] _localisedPopups;

        private readonly Dictionary<LocalisedPopupType, string> _popupTypeToLocalisationTerm = new Dictionary<LocalisedPopupType, string>();
        private readonly Queue<LocalisedPopupType> _popupQueue = new Queue<LocalisedPopupType>();

        private bool _isShowingPopup = false;
        private Coroutine _showHidePopupCoroutine;
        
        private void Awake()
        {
            RegenerateLocalisationTermsDictionary();
            
            _okayButton.onClick.AddListener(OkayButtonListener);

            ShowHidePopupUI(false, true);
        }

        private void OnDestroy()
        {
            _okayButton.onClick.RemoveListener(OkayButtonListener);
        }

        [ContextMenu(nameof(RegenerateLocalisationTermsDictionary))]
        private void RegenerateLocalisationTermsDictionary()
        {
            _popupTypeToLocalisationTerm.Clear();
            foreach (LocalisedPopup localisedPopup in _localisedPopups)
            {
                CircumDebug.Assert(!_popupTypeToLocalisationTerm.ContainsKey(localisedPopup.PopupType), $"Duplicate popup settings for popup type {localisedPopup.PopupType}");
                _popupTypeToLocalisationTerm.Add(localisedPopup.PopupType, localisedPopup.LocalisationTerm);
            }
        }
        
        public void EnqueueMessage(LocalisedPopupType popup)
        {
            _popupQueue.Enqueue(popup);
        }

        private void OkayButtonListener()
        {
            if (_popupQueue.Count == 0)
            {
                ShowHidePopupUI(false);
            }
            else
            {
                SetPopupMessage(_popupQueue.Dequeue());
            }
        }

        private void Update()
        {
            if (_isShowingPopup)
            {
                return;
            }

            if (_popupQueue.Count > 0)
            {
                SetPopupMessage(_popupQueue.Dequeue());
                ShowHidePopupUI(true);
            }
        }

        private void ShowHidePopupUI(bool show, bool instant = false)
        {
            if (_showHidePopupCoroutine != null)
            {
                StopCoroutine(_showHidePopupCoroutine);
            }

            _showHidePopupCoroutine = StartCoroutine(ShowHidePopupCoroutine(show, instant));
        }

        private IEnumerator ShowHidePopupCoroutine(bool show, bool instant)
        {
            if (show) _isShowingPopup = true;

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = true;
            
            float targetValue = show ? 1f : 0f;
            float transitionDuration = instant ? 0f : _showHideDuration;
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, targetValue, transitionDuration, SetOnAmount);

            _canvasGroup.interactable = show;
            _canvasGroup.blocksRaycasts = show;

            if (!show) _isShowingPopup = false;
        }

        private void SetOnAmount(float amount)
        {
            _canvasGroup.alpha = amount;
        }

        private void SetPopupMessage(LocalisedPopupType popupType)
        {
            string localisationTerm = _popupTypeToLocalisationTerm[popupType];
            string message = LeanLocalization.GetTranslationText(localisationTerm);
            _popupMessageText.text = message;
        }
    }
}