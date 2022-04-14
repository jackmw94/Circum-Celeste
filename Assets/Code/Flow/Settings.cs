using System;
using System.Collections;
using Code.Core;
using Code.Debugging;
using Code.Juice;
using Code.Level;
using Code.Level.Player;
using Code.UI;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityExtras.Code.Optional.Singletons;
using UnityExtras.Core;

namespace Code.Flow
{
    public class Settings : SingletonMonoBehaviour<Settings>
    {
        [SerializeField] private AnimateSettingsSection _animateGameOptionsSection;
        [SerializeField] private InterLevelScreen _interLevelScreen;
        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private CircumScreen _gameTips;
        [SerializeField] private CircumScreen _howToPlay;
        [SerializeField] private CircumScreen _addFriends;
        [Space(15)]
        [SerializeField] private Button _toggleSettingsButton;
        [SerializeField] private CanvasGroup _buttonCanvasGroup;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _toggleDuration = 0.25f;
        [Space(15)]
        [SerializeField] private Button _restartLevelButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _gameOptionsButton;
        [SerializeField] private Button _toggleFeedbacks;
        [SerializeField] private Button _toggleShowLevelTime;
        [SerializeField] private Button _gameTipsButton;
        [SerializeField] private Button _howToPlayButton;
        [SerializeField] private Button _changeQualityLevelButton;
        [SerializeField] private Button _addFriendsButton;
        [SerializeField] private AreYouSureButtonWrapper _resetStatsButton;
        [SerializeField] private TextMeshProUGUI _toggleFeedbacksLabel;
        [SerializeField] private TextMeshProUGUI _toggleShowLevelTimerLabel;
        [SerializeField] private TextMeshProUGUI _changeQualityLevelLabel;
        [Space(15)]
        [SerializeField] private NotificationUI _settingsNotifications;
        [SerializeField] private NotificationUI _friendsButtonNotifications;
        [Space(15)]
        [SerializeField, LeanTranslationName] private string _showLevelTimeLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _hideLevelTimeLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _turnFeedbacksOnLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _turnFeedbacksOffLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _changeQualitySettingLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _qualitySettingHighLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _qualitySettingMediumLocalisationTerm;

        private bool _settingsOn = false;
        private Coroutine _turnOnOffCoroutine = null;

        private CircumOptions CircumOptions => PersistentDataManager.Instance.Options;
        public bool SettingsAreShowing => _settingsOn;

        private void Awake()
        {
            _toggleSettingsButton.onClick.AddListener(SettingsButtonClicked);
            _backButton.onClick.AddListener(BackButtonListener);
            _restartLevelButton.onClick.AddListener(RestartLevel);
            _resetStatsButton.onClick.AddListener(ResetPlayerStats);
            _toggleFeedbacks.onClick.AddListener(ToggleFeedbacks);
            _toggleShowLevelTime.onClick.AddListener(ToggleShowLevelTimer);
            _gameTipsButton.onClick.AddListener(ShowTips);
            _howToPlayButton.onClick.AddListener(ShowHowToPlay);
            _gameOptionsButton.onClick.AddListener(ShowGameOptions);
            _changeQualityLevelButton.onClick.AddListener(ChangeQualityLevel);
            _addFriendsButton.onClick.AddListener(ShowAddFriends);

            TurnOffInstant();
        }

        private IEnumerator Start()
        {
            _buttonCanvasGroup.alpha = 0f;
            _buttonCanvasGroup.blocksRaycasts = false;
            
            yield return new WaitUntil(() => SceneManager.sceneCount == 1);
            
            _buttonCanvasGroup.blocksRaycasts = true;
            yield return Utilities.LerpOverTime(0f, 1f, 0.5f, f => _buttonCanvasGroup.alpha = f);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _toggleSettingsButton.onClick.RemoveListener(SettingsButtonClicked);
            _backButton.onClick.RemoveListener(BackButtonListener);
            _restartLevelButton.onClick.RemoveListener(RestartLevel);
            _resetStatsButton.onClick.RemoveListener(ResetPlayerStats);
            _toggleFeedbacks.onClick.RemoveListener(ToggleFeedbacks);
            _toggleShowLevelTime.onClick.RemoveListener(ToggleShowLevelTimer);
            _gameTipsButton.onClick.RemoveListener(ShowTips);
            _howToPlayButton.onClick.RemoveListener(ShowHowToPlay);
            _gameOptionsButton.onClick.RemoveListener(ShowGameOptions);
            _changeQualityLevelButton.onClick.RemoveListener(ChangeQualityLevel);
            _addFriendsButton.onClick.RemoveListener(ShowAddFriends);
        }

        private void ChangeQualityLevel()
        {
            CircumOptions.SetNextQualitySetting();
            CircumDebug.Log($"Set next quality setting to {CircumOptions.QualitySetting}");
            UpdateQualityLevelLabel();
            CircumOptions.Save(CircumOptions);
        }

        private void ShowGameOptions()
        {
            _animateGameOptionsSection.ShowSettingsSection();
        }

        private void OnSettingShowing()
        {
            _resetStatsButton.Reset();
            
            UpdateAllLabels();
        }

        private void UpdateAllLabels()
        {
            UpdateQualityLevelLabel();
            UpdateFeedbacksLabel();
            UpdateShowLevelTimerLabel();
        }

        private void SettingsButtonClicked()
        {
            if (!_animateGameOptionsSection.TopLevelCurrentlyShowing)
            {
                _animateGameOptionsSection.ShowTopSettings();
                return;
            }
            
            _settingsOn = !_settingsOn;
            TurnSettingsOnOff(_settingsOn);
        }

        private void ToggleShowLevelTimer()
        {
            CircumOptions circumOptions = CircumOptions;
            circumOptions.ShowLevelTimer = !circumOptions.ShowLevelTimer;
            UpdateShowLevelTimerLabel();
            
            GameContainer.Instance.LevelTimeUI.SettingsShowHideTime(circumOptions.ShowLevelTimer);
            CircumOptions.Save(circumOptions);
        }

        private void ShowAddFriends()
        {
            _friendsButtonNotifications.SetSeen();
            _addFriends.ShowHideScreen(true);
        }

        private void ShowTips()
        {
            _gameTips.ShowHideScreen(true);
        }
        
        private void ShowHowToPlay()
        {
            _howToPlay.ShowHideScreen(true);
        }

        private void UpdateShowLevelTimerLabel()
        {
            string localisationTerm = CircumOptions.ShowLevelTimer ? _hideLevelTimeLocalisationTerm : _showLevelTimeLocalisationTerm;
            string labelText = LeanLocalization.GetTranslationText(localisationTerm);
            _toggleShowLevelTimerLabel.text = labelText;
        }

        private void UpdateQualityLevelLabel()
        {
            string currentLevelLocalisationTerm = QualitySettingToLabelTerm(CircumOptions.QualitySetting);
            string currentLevelLabelText = LeanLocalization.GetTranslationText(currentLevelLocalisationTerm);
            string baseLabelText = LeanLocalization.GetTranslationText(_changeQualitySettingLocalisationTerm);
            
            string labelText = $"{baseLabelText} ({currentLevelLabelText})";
            _changeQualityLevelLabel.text = labelText;
        }

        private string QualitySettingToLabelTerm(CircumQuality.CircumQualitySetting qualitySetting)
        {
            switch (qualitySetting)
            {
                case CircumQuality.CircumQualitySetting.Medium: return _qualitySettingMediumLocalisationTerm;
                case CircumQuality.CircumQualitySetting.High: return _qualitySettingHighLocalisationTerm;
            }

            throw new ArgumentOutOfRangeException($"There is no localisation term for quality setting {qualitySetting}");
        }

        private void BackButtonListener()
        {
            _levelManager.ExitLevel();
            SettingsButtonClicked();
        }

        private void RestartLevel()
        {
            _levelManager.CreateCurrentLevel(transition: InterLevelFlow.InterLevelTransition.Fast);
            SettingsButtonClicked();
        }
        
        private void ResetPlayerStats()
        {
            PersistentDataManager.Instance.ResetStats();
            RemoteDataManager.Instance.ResetStats();
            _levelProvider.ResetToStart(true);
            _interLevelScreen.SetupInterLevelScreen();
        }
        
        private void ToggleFeedbacks()
        {
            bool feedbackActiveForUserSettings = Feedbacks.Instance.IsActiveForReason(ActiveState.ActiveReason.UserSetting);
            Feedbacks.Instance.SetActiveInactive(ActiveState.ActiveReason.UserSetting, !feedbackActiveForUserSettings);
            
            UpdateFeedbacksLabel();
        }

        private void UpdateFeedbacksLabel()
        {
            bool feedbackActiveForUserSettings = Feedbacks.Instance.IsActiveForReason(ActiveState.ActiveReason.UserSetting); 
            string localisationTerm = feedbackActiveForUserSettings ? _turnFeedbacksOffLocalisationTerm : _turnFeedbacksOnLocalisationTerm;
            string labelText = LeanLocalization.GetTranslationText(localisationTerm);
            _toggleFeedbacksLabel.text = labelText;
        }
        
        private void TurnSettingsOnOff(bool on)
        {
            if (on)
            {
                OnSettingShowing();
                _settingsNotifications.SetSeen();
            }
            else
            {
                _friendsButtonNotifications.SetSeen();
            }
            
            if (_turnOnOffCoroutine != null)
            {
                StopCoroutine(_turnOnOffCoroutine);
            }

            _turnOnOffCoroutine = StartCoroutine(TurnSettingsCanvasGroupOnOff(on));
        }

        private IEnumerator TurnSettingsCanvasGroupOnOff(bool on)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = false;
            
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, on ? 1f : 0f, _toggleDuration, f =>
            {
                _canvasGroup.alpha = f;
            });
            
            _canvasGroup.blocksRaycasts = on;
            _canvasGroup.interactable = on;
        }

        private void TurnOffInstant()
        {
            _settingsOn = false;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0f;
        }

        public void ShowNewFriendsNotification(int newFriendsCount)
        {
            _settingsNotifications.AddNotification(newFriendsCount);
            _friendsButtonNotifications.AddNotification(newFriendsCount);
        }
    }
}