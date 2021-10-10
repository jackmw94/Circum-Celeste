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
        [SerializeField] private TeaseScrollRect _teaseScrollRect;
        [Space(15)]
        [SerializeField] private Button _leftArrow;
        [SerializeField] private Button _rightArrow;
        [Space(15)]
        [SerializeField] private PositiveFeedbackScreen _positiveFeedbackScreen;
        [SerializeField] private LevelOverviewScreen _levelOverviewScreen;
        [SerializeField] private YourFastestTimesScreen _worldRecordsScreen;
        [SerializeField] private FriendsLevelRanking _friendsLevelRanking;
        [SerializeField] private RunOverviewScreen _runOverviewScreen;
        [Space(15)]
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
                int levelCount = _levelProvider.ActiveLevelProgression.LevelLayout.Length;
                int currentLevelIndex = currentLevel.LevelContext.LevelIndex;
                
                PlayerStats playerStats = PersistentDataManager.Instance.PlayerStats;
                int highestLevelPlayerReached = playerStats.HighestLevelIndex;

                _leftArrow.gameObject.SetActive(currentLevelIndex > 0);
                _rightArrow.gameObject.SetActive(currentLevelIndex < highestLevelPlayerReached && currentLevelIndex < levelCount - 1);
            }
        }

        public void SetupInterLevelScreen(BadgeData newBadgeData = new BadgeData(), NewFastestTimeInfo newFastestTimeInfo = null, bool showAdvanceLevelButtons = false)
        {
            LevelLayout levelLayout = _levelProvider.GetCurrentLevel();
            string levelName = levelLayout.name;
            float goldTime = levelLayout.GoldTime;

            PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
            LevelStats levelStats = persistentDataManager.GetStatsForLevelAtIndex(levelName);
            LevelRecording levelRecording = levelStats == null ? null : levelStats.HasRecording ? levelStats.LevelRecording : null;
            
            BadgeData currentBadgeData;
            if (levelRecording == null)
            {
                currentBadgeData = new BadgeData
                {
                    IsPerfect = false,
                    HasGoldTime = false,
                    HasPerfectGoldTime = false
                };
            }
            else
            {
                bool hasPerfect = levelRecording.RecordingData.IsPerfect;
                bool previouslyHasGoldTime = levelStats.HasPreviouslyCompletedInGoldTime;
                bool hasPerfectGold = hasPerfect && levelRecording.HasBeatenGoldTime(goldTime);
                currentBadgeData = new BadgeData
                {
                    IsPerfect = hasPerfect,
                    HasGoldTime = previouslyHasGoldTime,
                    HasPerfectGoldTime = hasPerfectGold
                };
            }

            _friendsLevelRanking.SetupRecordsScreen(levelLayout.name, levelLayout.GoldTime, ReplayLevel);
            
            _worldRecordsScreen.SetupRecordsScreen(levelLayout.GoldTime, currentBadgeData, levelRecording, ReplayLevel);

            _teaseScrollRect.enabled = !persistentDataManager.PlayerFirsts.SeenReplaysScreen && newFastestTimeInfo != null;
            _levelOverviewScreen.SetupLevelOverview(levelLayout, currentBadgeData, newBadgeData, newFastestTimeInfo, showAdvanceLevelButtons, PlayLevel, NextLevelButtonListener);
            
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

        public IEnumerator ShowHideScreen(bool show, InterLevelFlow.InterLevelTransition transition)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = false;
            
            float targetAlpha = show ? 1f : 0f;
            float duration = transition == InterLevelFlow.InterLevelTransition.Instant ? 0f : _showHideDuration;
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, targetAlpha, duration, f =>
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