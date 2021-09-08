using System;
using Code.Debugging;
using Code.Level;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class LevelOverviewScreen : MonoBehaviour
    {
        private const string LevelDurationTokenName = "LevelDuration";
        
        [SerializeField] private TextMeshProUGUI _levelNumber;
        [SerializeField] private TextMeshProUGUI _levelName;
        [SerializeField] private TextMeshProUGUI _levelTag;
        [FormerlySerializedAs("_perfectIcon")]
        [Space(15)]
        [SerializeField] private LevelBadge _levelBadge;
        [Space(15)]
        [SerializeField] private Button[] _playButtons;
        [SerializeField] private Button _advanceButton;
        [Space(15)]
        [SerializeField] private GameObject _playButtonRoot;
        [SerializeField] private GameObject _playAndAdvanceButtonRoot;

        private Action _playLevelCallback = null;
        private Action _advanceLevelCallback = null;

        private void Awake()
        {
            _playButtons.ApplyFunction(p => p.onClick.AddListener(PlayButtonClicked));
            _advanceButton.onClick.AddListener(AdvanceButtonClicked);
        }

        private void OnDestroy()
        {
            _playButtons.ApplyFunction(p => p.onClick.RemoveListener(PlayButtonClicked));
            _advanceButton.onClick.RemoveListener(AdvanceButtonClicked);
        }

        public void SetupLevelOverview(LevelLayout levelLayout, bool isPerfect, bool isFirstPerfect, bool showAdvancePrompt, Action playLevelCallback, Action advanceLevelCallback)
        {
            int levelNumber = levelLayout.LevelContext.LevelNumber;
            
            _levelNumber.text = $"{levelNumber}";
            _levelNumber.enabled = levelNumber > 0;
            
            _levelName.text = levelLayout.name;

            SetTagString(levelLayout);
            
            _playButtonRoot.SetActiveSafe(!showAdvancePrompt);
            _playAndAdvanceButtonRoot.SetActiveSafe(showAdvancePrompt);

            CircumDebug.Assert(isPerfect || !isFirstPerfect, "Arguments say this level was NOT perfect but WAS the first perfect. Unexpected.");
            _levelBadge.ShowHideBadge(isPerfect, !isFirstPerfect);
            
            _playLevelCallback = playLevelCallback;
            _advanceLevelCallback = advanceLevelCallback;
        }

        private void SetTagString(LevelLayout levelLayout)
        {
            string escapeTimeString = Mathf.RoundToInt(levelLayout.EscapeTimer).ToString();
            Lean.Localization.LeanLocalization.SetToken(LevelDurationTokenName, escapeTimeString);
            
            string tagText = Lean.Localization.LeanLocalization.GetTranslationText(levelLayout.TagLineLocalisationTerm, "");
            CircumDebug.Assert(string.IsNullOrEmpty(levelLayout.TagLineLocalisationTerm) == string.IsNullOrEmpty(tagText), $"Error getting localisation term {levelLayout.TagLineLocalisationTerm} (localised='{tagText}')");
            
            _levelTag.text = tagText;
        }
        
        private void PlayButtonClicked()
        {
            _playLevelCallback();
        }

        private void AdvanceButtonClicked()
        {
            _advanceLevelCallback();
        }
    }
}