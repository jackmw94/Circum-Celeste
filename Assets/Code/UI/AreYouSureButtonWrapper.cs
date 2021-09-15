using System;
using System.Diagnostics;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    [RequireComponent(typeof(Button))]
    public class AreYouSureButtonWrapper : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField, LeanTranslationName] private string _regularTextLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _areYouSureTextLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _completeTextLocalisationTerm;

        private bool _showingAreYouSure = false;
        
        // ReSharper disable once InconsistentNaming - matches UI.Button naming 
        public Button.ButtonClickedEvent onClick { get; set; } = new Button.ButtonClickedEvent();
        
        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_button)
            {
                _button = GetComponent<Button>();
            }
        }
        
        private void Awake()
        {
            _button.onClick.AddListener(ButtonClickedListener);
        }

        private void Start()
        {
            Reset();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ButtonClickedListener);
        }

        private void ButtonClickedListener()
        {
            if (!_showingAreYouSure)
            {
                ShowHideAreYouSure(true, false);
                return;
            }

            onClick.Invoke();
            ShowHideAreYouSure(false, true);
        }

        public void Reset()
        {
            ShowHideAreYouSure(false, false);
        }

        private void ShowHideAreYouSure(bool showAreYouSure, bool actionComplete)
        {
            _showingAreYouSure = showAreYouSure;

            string localisation = showAreYouSure switch
            {
                true => _areYouSureTextLocalisationTerm,
                false when actionComplete => _completeTextLocalisationTerm,
                false => _regularTextLocalisationTerm
            };

            _label.text = LeanLocalization.GetTranslationText(localisation);
        }
    }
}