using System.Collections;
using TMPro;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class NotificationUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private float _showHideDuration = 0.25f;

        private Coroutine _showHideCoroutine = null;
        private bool _isShowing = false;

        protected int NotificationCount { private set; get; } = 0;
        private bool ShouldShow => NotificationCount > 0;

        private void Awake()
        {
            UpdateNotification(true);
        }
        
        public virtual void SetSeen()
        {
            NotificationCount = 0;
            UpdateNotification();
        }
        
        public void AddNotification(int count)
        {
            count = Mathf.Max(count, 0);
            NotificationCount += count;
            UpdateNotification();
        }

        private void UpdateNotification(bool forceInstant = false)
        {
            if (ShouldShow)
            {
                _label.text = NotificationCount.ToString();
            }

            if (_isShowing == ShouldShow && !forceInstant)
            {
                return;
            }
            
            _isShowing = ShouldShow;
            
            if (_showHideCoroutine != null)
            {
                StopCoroutine(_showHideCoroutine);
            }
            _showHideCoroutine = StartCoroutine(ShowHideCanvasCoroutine(forceInstant));
        }

        private IEnumerator ShowHideCanvasCoroutine(bool instant)
        {
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, ShouldShow ? 1f : 0f, instant ? 0f : _showHideDuration, f =>
            {
                _canvasGroup.alpha = f;
            });
        }
    }
}