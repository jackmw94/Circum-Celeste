using System.Collections;
using Code.Core;
using Code.Juice;
using Code.Level;
using Code.Level.Player;
using Code.UI;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class Settings : MonoBehaviour
    {
        private const string UpdateRemoteConfigDefaultText = "Update game configuration";
        
        [SerializeField] private InterLevelScreen _interLevelScreen;
        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private LevelManager _levelManager;
        [Space(15)]
        [SerializeField] private Button _toggleSettingsButton;
        [SerializeField] private CanvasGroup _buttonCanvasGroup;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _toggleDuration = 0.25f;
        [Space(15)]
        [SerializeField] private Button _restartLevelButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _updateRemoteConfigButton;
        [SerializeField] private Button _toggleFeedbacks;
        [SerializeField] private Button _toggleShowLevelTime;
        [SerializeField] private Button _toggleNoLoadingSaving;
        [SerializeField] private AreYouSureButtonWrapper _resetStatsButton;
        [SerializeField] private Button _resetSplashScreens;
        [SerializeField] private TextMeshProUGUI _updateRemoteConfigLabel;
        [SerializeField] private TextMeshProUGUI _toggleFeedbacksLabel;
        [SerializeField] private TextMeshProUGUI _toggleNoLoadingSavingLabel;
        [SerializeField] private TextMeshProUGUI _toggleShowLevelTimerLabel;
        [Space(15)]
        [SerializeField, LeanTranslationName] private string _showLevelTimeLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _hideLevelTimeLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _turnFeedbacksOnLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _turnFeedbacksOffLocalisationTerm;

        private bool _settingsOn = false;
        private Coroutine _turnOnOffCoroutine = null;

        private CircumOptions CircumOptions => PersistentDataManager.Instance.Options;

        private void Awake()
        {
            _toggleSettingsButton.onClick.AddListener(SettingsButtonClicked);
            _backButton.onClick.AddListener(BackButtonListener);
            _restartLevelButton.onClick.AddListener(RestartLevel);
            _updateRemoteConfigButton.onClick.AddListener(UpdateRemoteConfigButtonListener);
            _resetStatsButton.onClick.AddListener(ResetPlayerStats);
            _toggleFeedbacks.onClick.AddListener(ToggleFeedbacks);
            _toggleShowLevelTime.onClick.AddListener(ToggleShowLevelTimer);
            _toggleNoLoadingSaving.onClick.AddListener(ToggleNoLoadingSaving);

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

        private void OnDestroy()
        {
            _toggleSettingsButton.onClick.RemoveListener(SettingsButtonClicked);
            _backButton.onClick.RemoveListener(BackButtonListener);
            _restartLevelButton.onClick.RemoveListener(RestartLevel);
            _updateRemoteConfigButton.onClick.RemoveListener(UpdateRemoteConfigButtonListener);
            _resetStatsButton.onClick.RemoveListener(ResetPlayerStats);
            _toggleFeedbacks.onClick.RemoveListener(ToggleFeedbacks);
            _toggleShowLevelTime.onClick.RemoveListener(ToggleShowLevelTimer);
            _toggleNoLoadingSaving.onClick.RemoveListener(ToggleNoLoadingSaving);
        }
        
        private void OnSettingShowing()
        {
            _updateRemoteConfigLabel.text = UpdateRemoteConfigDefaultText;
            
            _resetStatsButton.Reset();
            
            UpdateFeedbacksLabel();
            UpdateNoLoadingSavingLabel();
            UpdateShowLevelTimerLabel();
        }

        private void SettingsButtonClicked()
        {
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

        private void UpdateShowLevelTimerLabel()
        {
            string localisationTerm = CircumOptions.ShowLevelTimer ? _hideLevelTimeLocalisationTerm : _showLevelTimeLocalisationTerm;
            string labelText = LeanLocalization.GetTranslationText(localisationTerm);
            _toggleShowLevelTimerLabel.text = labelText;
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
        
        private void UpdateRemoteConfigButtonListener()
        {
            _updateRemoteConfigButton.interactable = false;
            RemoteConfigHelper.RequestRefresh(success =>
            {
                _updateRemoteConfigButton.interactable = true;
                _updateRemoteConfigLabel.text = $"{UpdateRemoteConfigDefaultText} - was {(success ? "successful" : "unsuccessful")}";
            });
        }

        private void ResetPlayerStats()
        {
            PersistentDataManager.Instance.ResetStats();
            _levelProvider.ResetToStart(true);
            _interLevelScreen.SetupInterLevelScreen();
        }
        
        private void ToggleFeedbacks()
        {
            Feedbacks.Instance.FeedbacksActive = !Feedbacks.Instance.FeedbacksActive;
            UpdateFeedbacksLabel();
        }

        private void UpdateFeedbacksLabel()
        {
            string localisationTerm = Feedbacks.Instance.FeedbacksActive ? _turnFeedbacksOffLocalisationTerm : _turnFeedbacksOnLocalisationTerm;
            string labelText = LeanLocalization.GetTranslationText(localisationTerm);
            _toggleFeedbacksLabel.text = labelText;
        }

        private void ToggleNoLoadingSaving()
        {
            PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
            persistentDataManager.SetDoNotLoadOrSave(!persistentDataManager.DoNotLoadOrSave);
            UpdateNoLoadingSavingLabel();
        }

        private void UpdateNoLoadingSavingLabel()
        {
            _toggleNoLoadingSavingLabel.text = $"Toggle persistent data ({(PersistentDataManager.Instance.DoNotLoadOrSave ? "saving / loading DISABLED" : "saving / loading on")})";
        }

        private void TurnSettingsOnOff(bool on)
        {
            if (on)
            {
                OnSettingShowing();
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
    }
}