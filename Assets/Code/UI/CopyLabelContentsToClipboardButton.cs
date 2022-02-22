using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Core;

namespace Code.UI
{
    public class CopyLabelContentsToClipboardButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [Space(15)]
        [SerializeField] private float _showCopyCompletedForTime = 5f;
        [SerializeField] private GameObject _copyCompleted;

        private Coroutine _showCopyCompletedCoroutine = null;
        
        private void Awake()
        {
            _button.onClick.AddListener(CopyLabelContents);
        }

        private void OnEnable()
        {
            if (_copyCompleted)
            {
                _copyCompleted.SetActiveSafe(false);
            }
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(CopyLabelContents);
        }

        private void CopyLabelContents()
        {
            UniClipboard.SetText(_textMeshProUGUI.text);

            if (_copyCompleted)
            {
                if (_showCopyCompletedCoroutine != null)
                {
                    StopCoroutine(_showCopyCompletedCoroutine);
                }
                _showCopyCompletedCoroutine = StartCoroutine(ShowCopyCompleted());
            }
        }

        private IEnumerator ShowCopyCompleted()
        {
            _copyCompleted.SetActive(true);
            yield return new WaitForSeconds(_showCopyCompletedForTime);
            _copyCompleted.SetActive(false);
        }
    }
}