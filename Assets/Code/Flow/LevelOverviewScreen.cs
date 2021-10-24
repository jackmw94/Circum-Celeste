using System;
using Code.Debugging;
using Code.Level;
using Code.Level.Player;
using Code.UI;
using Code.VFX;
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
        [Space(15)]
        [SerializeField] private BadgeIndicator _badgeIndicator;
        [Space(15)] 
        [SerializeField] private Color _regularNewFastestTimeLabelColour;
        [SerializeField] private Color _perfectNewFastestTimeLabelColour;
        [SerializeField] private GameObject _newFastestTimeRoot;
        [SerializeField] private TextMeshProUGUI _newFastestTimeLabel;
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

        public void SetupLevelOverview(InterLevelFlow.InterLevelFlowSetupData interLevelFlowSetupData, LevelLayout levelLayout, BadgeData currentBadgeData, Action playLevelCallback, Action advanceLevelCallback)
        {
            int levelNumber = levelLayout.LevelContext.LevelNumber;
            
            _levelNumber.text = $"{levelNumber}";
            _levelNumber.enabled = levelNumber > 0;
            
            _levelName.text = levelLayout.name;

            SetTagString(levelLayout);
            
            _playButtonRoot.SetActiveSafe(!interLevelFlowSetupData.HasComeFromLevelCompletion);
            _playAndAdvanceButtonRoot.SetActiveSafe(interLevelFlowSetupData.HasComeFromLevelCompletion);

            NewFastestTimeInfo newFastestTimeInfo = interLevelFlowSetupData.NewFastestTimeInfo;
            bool hasNewFastestTime = newFastestTimeInfo != null;
            _newFastestTimeRoot.SetActive(hasNewFastestTime);
            if (hasNewFastestTime)
            {
                _newFastestTimeLabel.text = $"{newFastestTimeInfo.Time:0.00}";
                _newFastestTimeLabel.color = newFastestTimeInfo.IsPerfect ? _perfectNewFastestTimeLabelColour : _regularNewFastestTimeLabelColour;
            }

            _badgeIndicator.SetupBadgeIndicator(currentBadgeData, interLevelFlowSetupData.NewBadgeData, CheckGameComplete);
            
            _playLevelCallback = playLevelCallback;
            _advanceLevelCallback = advanceLevelCallback;
        }

        private void CheckGameComplete()
        {
            PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
            if (!persistentDataManager.PlayerFirsts.CompletedGame && persistentDataManager.HasCompletedGame())
            {
                Popup.Instance.EnqueueMessage(Popup.LocalisedPopupType.CompletedGame);
                persistentDataManager.PlayerFirsts.CompletedGame = true;
                PlayerFirsts.Save(persistentDataManager.PlayerFirsts);
                
                AppFeedbacks.Instance.TriggerComets();
            }
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