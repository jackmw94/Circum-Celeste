﻿using System.Collections;
using Code.Core;
using Code.Juice;
using Code.Level;
using Code.Level.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class Settings : MonoBehaviour
    {
        private const string UpdateRemoteConfigDefaultText = "Update game configuration";

        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private PlayerStatsManager _playerStatsManager;
        [SerializeField] private LevelManager _levelManager;
        [Space(15)]
        [SerializeField] private Button _toggleSettingsButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _toggleDuration = 0.25f;
        [Space(15)]
        [SerializeField] private Button _restartLevelButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _updateRemoteConfigButton;
        [SerializeField] private Button _resetStatsButton;
        [SerializeField] private Button _resetTutorialsButton;
        [SerializeField] private Button _deleteSaveDataButton;
        [SerializeField] private Button _toggleFeedbacks;
        [SerializeField] private TextMeshProUGUI _toggleFeedbacksLabel;
        [SerializeField] private TextMeshProUGUI _updateRemoteConfigLabel;

        private bool _settingsOn = false;
        private Coroutine _turnOnOffCoroutine = null;

        private void Awake()
        {
            _toggleSettingsButton.onClick.AddListener(SettingsButtonClicked);
            _backButton.onClick.AddListener(BackButtonListener);
            _restartLevelButton.onClick.AddListener(RestartLevel);
            _updateRemoteConfigButton.onClick.AddListener(UpdateRemoteConfigButtonListener);
            _resetStatsButton.onClick.AddListener(ResetPlayerStats);
            _resetTutorialsButton.onClick.AddListener(ResetTutorials);
            _toggleFeedbacks.onClick.AddListener(ToggleFeedbacks);
            _deleteSaveDataButton.onClick.AddListener(DeleteSaveData);

            TurnOffInstant();
        }

        private void OnDestroy()
        {
            _toggleSettingsButton.onClick.RemoveListener(SettingsButtonClicked);
            _backButton.onClick.RemoveListener(BackButtonListener);
            _restartLevelButton.onClick.RemoveListener(RestartLevel);
            _updateRemoteConfigButton.onClick.RemoveListener(UpdateRemoteConfigButtonListener);
            _resetStatsButton.onClick.RemoveListener(ResetPlayerStats);
            _resetTutorialsButton.onClick.RemoveListener(ResetTutorials);
            _toggleFeedbacks.onClick.RemoveListener(ToggleFeedbacks);
            _deleteSaveDataButton.onClick.RemoveListener(DeleteSaveData);
        }
        
        private void OnSettingShowing()
        {
            _updateRemoteConfigLabel.text = UpdateRemoteConfigDefaultText;
        }

        private void SettingsButtonClicked()
        {
            _settingsOn = !_settingsOn;
            TurnSettingsOnOff(_settingsOn);
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
            PlayerStats.ResetSavedPlayerStats();
        }

        private void ResetTutorials()
        {
            _playerStatsManager.ResetTutorials();
        }

        private void ToggleFeedbacks()
        {
            Feedbacks.Instance.FeedbacksActive = !Feedbacks.Instance.FeedbacksActive;
            _toggleFeedbacksLabel.text = $"Toggle Feedbacks ({(Feedbacks.Instance.FeedbacksActive ? "on" : "off")})";
        }

        private void DeleteSaveData()
        {
            _playerStatsManager.PlayerStats.ResetSaveData();
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