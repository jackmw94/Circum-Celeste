using System.Diagnostics;
using Code.Core;
using Code.Level.Player;
using TMPro;
using UnityCommonFeatures;
using UnityEngine;

namespace Code.Debugging
{
    public class DebugToggleDisableLeaderboardUpdates : ButtonBehaviour
    {
        [SerializeField] private TextMeshProUGUI _buttonLabel;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!_buttonLabel)
            {
                _buttonLabel = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            _buttonLabel.text = $"{(PersistentDataManager.Instance.Options.DisableLeaderboardUpdates ? "Enable" : "Disable")} leaderboards update";
        }
        
        protected override void OnButtonClicked()
        {
            PersistentDataManager.Instance.Options.DisableLeaderboardUpdates = !PersistentDataManager.Instance.Options.DisableLeaderboardUpdates;
            CircumOptions.Save(PersistentDataManager.Instance.Options);
            UpdateLabel();
        }
    }
}