using System.Collections;
using Code.Debugging;
using Code.Level;
using Code.Level.Player;
using Code.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class InterLevelScreen : MonoBehaviour
    {
        [SerializeField] private float _showHideDuration = 0.5f;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ScrollingItemPicker _scrollingItemPicker;
        [Space(15)]
        [SerializeField] private Button _leftArrow;
        [SerializeField] private Button _rightArrow;
        [Space(15)]
        [SerializeField] private PositiveFeedbackScreen _positiveFeedbackScreen;
        [SerializeField] private LevelOverviewScreen _levelOverviewScreen;
        [SerializeField] private WorldRecordsScreen _worldRecordsScreen;
        [SerializeField] private RunOverviewScreen _runOverviewScreen;
        [Space(15)]
        [SerializeField] private PlayerStatsManager _playerStatsManager;
        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private LevelManager _levelManager;

        private void Awake()
        {
            _leftArrow.onClick.AddListener(PreviousLevelButtonListener);
            _rightArrow.onClick.AddListener(NextLevelButtonListener);
        }

        private void OnDestroy()
        {
            _leftArrow.onClick.RemoveListener(PreviousLevelButtonListener);
            _rightArrow.onClick.RemoveListener(NextLevelButtonListener);
        }

        private void PreviousLevelButtonListener()
        {
            _levelProvider.PreviousLevel();
            SetupInterLevelScreen();
        }

        private void NextLevelButtonListener()
        {
            _levelProvider.AdvanceLevel();
            SetupInterLevelScreen();
        }

        private void SetNextPreviousButtonsActive()
        {
            LevelLayout currentLevel = _levelProvider.GetCurrentLevel();
            if (currentLevel.LevelContext.IsTutorial)
            {
                _leftArrow.gameObject.SetActive(false);
                _rightArrow.gameObject.SetActive(false);
            }
            else
            {
                _leftArrow.gameObject.SetActive(currentLevel.LevelContext.LevelIndex > 0);
                _rightArrow.gameObject.SetActive(currentLevel.LevelContext.LevelIndex < _playerStatsManager.PlayerStats.HighestLevelIndex);
            }
        }

        public void SetupInterLevelScreen(bool isFirstPerfect = false, bool showAdvanceLevelButtons = false)
        {
            LevelLayout levelLayout = _levelProvider.GetCurrentLevel();

            PlayerStats playerStats = _playerStatsManager.PlayerStats;
            LevelRecording levelRecording = playerStats.GetRecordingForLevelAtIndex(levelLayout.LevelContext.LevelIndex, false);
            LevelRecording perfectLevelRecording = playerStats.GetRecordingForLevelAtIndex(levelLayout.LevelContext.LevelIndex, true);
            _worldRecordsScreen.SetupRecordsScreen(levelRecording, perfectLevelRecording, ReplayLevel);
            
            _levelOverviewScreen.SetupLevelOverview(levelLayout, perfectLevelRecording != null, isFirstPerfect,showAdvanceLevelButtons, PlayLevel, NextLevelButtonListener);
            
            _scrollingItemPicker.SetToItemAtIndex(_scrollingItemPicker.NumberOfItems - 1);
            _scrollingItemPicker.SetScrollingEnabled(!levelLayout.LevelContext.IsTutorial);

            SetNextPreviousButtonsActive();
        }

        private void PlayLevel()
        {
            _levelManager.CreateCurrentLevel();
        }

        private void ReplayLevel(LevelRecording levelRecording)
        {
            _levelManager.CreateCurrentLevel(levelRecording);
        }

        public IEnumerator ShowHideScreen(bool show)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = false;
            
            float targetAlpha = show ? 1f : 0f;
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, targetAlpha, _showHideDuration, f =>
            {
                _canvasGroup.alpha = f;
            });
            
            _canvasGroup.blocksRaycasts = show;
            _canvasGroup.interactable = show;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(EditorKeyCodeBindings.PreviousLevel))
            {
                PreviousLevelButtonListener();
            }

            if (Input.GetKeyDown(EditorKeyCodeBindings.NextLevel))
            {
                NextLevelButtonListener();
            }
        }
#endif
    }
}