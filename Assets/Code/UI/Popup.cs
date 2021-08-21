using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.Singletons;

namespace Code.UI
{
    public class Popup : SingletonMonoBehaviour<Popup>
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _okayButton;
        [SerializeField] private TextMeshProUGUI _popupMessageText;
        [Space(15)]
        [SerializeField] private float _showHideDuration = 0.25f;

        private readonly Queue<string> _popupQueue = new Queue<string>();

        private bool _isShowingPopup = false;
        private Coroutine _showHidePopupCoroutine;
        
        private void Awake()
        {
            _okayButton.onClick.AddListener(OkayButtonListener);
            
            ShowHidePopupUI(false, true);
        }

        private void OnDestroy()
        {
            _okayButton.onClick.RemoveListener(OkayButtonListener);
        }

        [ContextMenu(nameof(DebugEnqueueMessage))]
        public void DebugEnqueueMessage()
        {
            EnqueueMessage($"Hello {Random.Range(0, 100)}");
        }

        public void EnqueueMessage(string message)
        {
            _popupQueue.Enqueue(message);
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

        private void SetPopupMessage(string message)
        {
            _popupMessageText.text = message;
        }
    }
}