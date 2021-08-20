using System.Collections;
using Code.Core;
using Code.Level;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private Button _toggleSettingsButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _toggleDuration = 0.25f;
        [Space(15)]
        [SerializeField] private Button _nextLevelButton;

        private bool _settingsOn = false;
        private Coroutine _turnOnOffCoroutine = null;

        private void Awake()
        {
            _toggleSettingsButton.onClick.AddListener(SettingsButtonClicked);
            _nextLevelButton.onClick.AddListener(NextLevelButtonListener);

            TurnOffInstant();
        }

        private void OnDestroy()
        {
            _toggleSettingsButton.onClick.RemoveListener(SettingsButtonClicked);
            _nextLevelButton.onClick.RemoveListener(NextLevelButtonListener);
        }

        private void SettingsButtonClicked()
        {
            _settingsOn = !_settingsOn;
            TurnSettingsOnOff(_settingsOn);
        }

        private void NextLevelButtonListener()
        {
            GameContainer gameContainer = GameContainer.Instance;
            LevelManager levelManager = gameContainer.LevelManager;
            levelManager.GenerateNextLevel();
            SettingsButtonClicked();
        }

        private void TurnSettingsOnOff(bool on)
        {
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