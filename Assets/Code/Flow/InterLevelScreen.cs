using System.Collections;
using Code.Debugging;
using Code.Level;
using Code.Level.Player;
using Code.UI;
using Code.VFX;
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
        [SerializeField] private LevelOverviewScreen _levelOverviewScreen;
        [SerializeField] private YourFastestTimesScreen _worldRecordsScreen;
        [SerializeField] private PlayerLevelRankingPanel _friendsLevelRankingScreen;
        [SerializeField] private PlayerLevelRankingPanel _globalLevelRankingScreen;
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

        public void SetupInterLevelScreen(InterLevelFlow.InterLevelFlowSetupData interLevelFlowSetupData = null)
        {
            interLevelFlowSetupData ??= new InterLevelFlow.InterLevelFlowSetupData();
            LevelLayout levelLayout = _levelProvider.GetCurrentLevel();
            string levelName = levelLayout.name;
            float goldTime = levelLayout.GoldTime;

            PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
            LevelStats levelStats = persistentDataManager.GetStatsForLevelAtIndex(levelName);
            LevelRecording levelRecording = levelStats == null ? null : levelStats.HasRecording ? levelStats.LevelRecording : null;
            
            BadgeData currentBadgeData;
            BadgeData bestLevelBadgeData;
            if (levelRecording == null || levelLayout.LevelContext.IsTutorial)
            {
                currentBadgeData = default;
                bestLevelBadgeData = default;
            }
            else
            {
                bool hasPerfect = levelRecording.RecordingData.IsPerfect;
                bool previouslyHasGoldTime = levelStats.HasPreviouslyCompletedInGoldTime;
                bool bestLevelHasGoldTime = levelRecording.HasBeatenGoldTime(goldTime);
                bool hasPerfectGold = hasPerfect && bestLevelHasGoldTime;
                currentBadgeData = new BadgeData
                {
                    IsPerfect = hasPerfect,
                    HasGoldTime = previouslyHasGoldTime,
                    HasPerfectGoldTime = hasPerfectGold
                };
                bestLevelBadgeData = new BadgeData()
                {
                    IsPerfect = hasPerfect,
                    HasGoldTime = bestLevelHasGoldTime,
                    HasPerfectGoldTime = hasPerfectGold
                };
            }
            
            if (interLevelFlowSetupData.LevelGotPerfect)
            {
                AppFeedbacks.Instance.TriggerMenuEdgeBurst();
            }

            _friendsLevelRankingScreen.SetupRecordsScreen(levelLayout.name, levelLayout.GoldTime, ReplayLevel);
            _globalLevelRankingScreen.SetupRecordsScreen(levelLayout.name, levelLayout.GoldTime, ReplayLevel);
            
            _worldRecordsScreen.SetupRecordsScreen(levelLayout.GoldTime, bestLevelBadgeData, levelRecording, ReplayLevel);

            _teaseScrollRect.enabled = !persistentDataManager.PlayerFirsts.SeenReplaysScreen && interLevelFlowSetupData.NewFastestTimeInfo != null;
            _levelOverviewScreen.SetupLevelOverview(interLevelFlowSetupData, levelLayout, currentBadgeData, PlayLevel, NextLevelButtonListener);
            
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