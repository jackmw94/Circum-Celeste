using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class LinkButton : MonoBehaviour
    {
        [SerializeField] private Button _linkButton;
        [SerializeField] private string _linkUrl;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_linkButton)
            {
                _linkButton = GetComponent<Button>();
            }
        }

        private void Awake()
        {
            _linkButton.onClick.AddListener(LinkButtonClicked);
        }

        private void OnDestroy()
        {
            _linkButton.onClick.RemoveListener(LinkButtonClicked);
        }

        private void LinkButtonClicked()
        {
            Application.OpenURL(_linkUrl);
        }
    }
}