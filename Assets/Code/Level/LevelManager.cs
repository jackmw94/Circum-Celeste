using System;
using System.Collections;
using Code.Core;
using Code.Debugging;
using Code.Flow;
using Code.Level.Player;
using Code.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityExtras.Core;

namespace Code.Level
{
    public class LevelManager : MonoBehaviour
    {
        private const float ShowHowToPlayPopUpDelay = 1.33f;
        private const float CheckGlobalLevelRankingDelay = 1f;
        
        [SerializeField] private InterLevelFlow _interLevelFlow;
        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private PlayerContainer _playerContainer;
        [Space(15)]
        [SerializeField] private LevelGenerator _levelGenerator;
        [Space(30)]
        [SerializeField] private bool _dontUpdateLevelRankingsInEditor = true;

        private Coroutine _startLevelOnceMovedCoroutine = null;
        
        public LevelInstanceBase CurrentLevelInstance { get; private set; }

        public void CreateChallengeLevel(ChallengeLevel challengeLevel, Action<LevelResult> onChallengeCompleted)
        {
            if (CurrentLevelInstance)
            {
                Destroy(CurrentLevelInstance);
            }
            
            if (!_interLevelFlow.IsOverlaid)
            {
                _interLevelFlow.ShowHideUI(() =>
                {
                    CreateLevelInternal(challengeLevel.LevelLayout, null, onChallengeCompleted);
                }, InterLevelFlow.InterLevelTransition.Regular);
            }
            else
            {
                CreateLevelInternal(challengeLevel.LevelLayout, null, onChallengeCompleted);
            }
        }
        
        public void CreateCurrentLevel(LevelRecording replay = null, InterLevelFlow.InterLevelTransition transition = InterLevelFlow.InterLevelTransition.Regular)
        {
            if (replay != null)
            {
                LevelLayoutContext levelContext = _levelProvider.GetCurrentLevel().LevelContext;
                AnalyticsHelper.LevelStartedEvent(levelContext.LevelNumber);
            }
            
            if (CurrentLevelInstance)
            {
                Destroy(CurrentLevelInstance);
            }
            
            LevelLayout levelLayout = _levelProvider.GetCurrentLevel();
            
            if (!_interLevelFlow.IsOverlaid)
            {
                _interLevelFlow.ShowHideUI(() =>
                {
                    CreateLevelInternal(levelLayout, replay, OnLevelFinished);
                }, transition);
            }
            else
            {
                CreateLevelInternal(levelLayout, replay, OnLevelFinished);
            }
        }

        public void ExitLevel()
        {
            if (CurrentLevelInstance)
            {
                Destroy(CurrentLevelInstance);
            }
            
            if (!_levelProvider.GetCurrentLevel().LevelContext.IsFirstLevel)
            {
                PersistentDataManager.Instance.SetPlayerDied();
            }

            if (_startLevelOnceMovedCoroutine != null)
            {
                StopCoroutine(_startLevelOnceMovedCoroutine);
            }

            _interLevelFlow.ShowInterLevelUI(new InterLevelFlow.InterLevelFlowSetupData
            {
                Transition = InterLevelFlow.InterLevelTransition.Fast,
                OnShown = ClearCurrentLevel,
                ShowingTutorial = _levelProvider.GetCurrentLevel().LevelContext.IsTutorial
            });
        }

        private void ClearCurrentLevel()
        {
            _levelGenerator.DestroyLevel();
        }

        private void CreateLevelInternal(LevelLayout levelLayout, LevelRecording replay, Action<LevelResult> onLevelFinished)
        {
            bool isReplay = replay != null;
            if (isReplay)
            {
                CurrentLevelInstance = _levelGenerator.GenerateReplay(replay.RecordingData, levelLayout);
            }
            else
            {
                InputProvider[] inputProviders = _playerContainer.GetPlayerInputProviders();
                CurrentLevelInstance = _levelGenerator.GenerateLevel(inputProviders, levelLayout);
            }

            if (levelLayout.LevelContext.IsFirstLevel)
            {
                PersistentDataManager.Instance.ResetCurrentRun();
            }
            
            PersistentDataManager.Instance.SetLevelIndex(levelLayout.LevelContext.LevelIndex, true);

            if (_startLevelOnceMovedCoroutine != null)
            {
                StopCoroutine(_startLevelOnceMovedCoroutine);
            }
            _startLevelOnceMovedCoroutine = StartCoroutine(StartLevelWhenReady(onLevelFinished));
        }
        
        private IEnumerator StartLevelWhenReady(Action<LevelResult> onLevelFinished)
        {
            _interLevelFlow.HideInterLevelUI();
            yield return new WaitUntil(() => !_interLevelFlow.IsOverlaid);
            
            CurrentLevelInstance.LevelReady();
            
            yield return new WaitUntil(() => CurrentLevelInstance.PlayerStartedPlaying);

            CircumDebug.Log($"Level '{CurrentLevelInstance.name}' has started");
            CurrentLevelInstance.StartLevel(onLevelFinished);
        }
        
        public void OnLevelFinished(LevelResult levelResult)
        {
            bool isReplay = levelResult.WasReplay;
            bool hasComeFromLevelCompletion = false;
            bool completedPerfectLevel = false;
            PersistentDataManager.UpdatedStatisticsData updatedStatisticsData = null;

            if (!isReplay)
            {
                PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
                LevelLayout currentLevel = _levelProvider.GetCurrentLevel();
                
                if (levelResult.Success)
                {
                    LevelLayoutContext levelContext = currentLevel.LevelContext;
                    LevelRecordingData levelRecordingData = levelResult.LevelRecordingData;

                    LevelRecording levelRecording = new LevelRecording
                    {
                        LevelIndex = levelContext.LevelIndex,
                        RecordingData = levelRecordingData
                    };

                    int levelNumber = levelContext.LevelNumber;
                    bool isPerfect = levelRecordingData.IsPerfect;
                    bool beatGoldTime = levelRecording.HasBeatenGoldTime(currentLevel.GoldTime);
                    AnalyticsHelper.LevelCompletedEvent(levelNumber, isPerfect, beatGoldTime);

                    completedPerfectLevel = isPerfect && beatGoldTime;

                    updatedStatisticsData = persistentDataManager.UpdateStatisticsAfterLevel(currentLevel, levelRecording);

                    if (updatedStatisticsData.ReplacedPreviousLevelRecord)
                    {
                        StartCoroutine(CheckIfBeatGlobalRecordAfterDelay(currentLevel.name));
                    }
                    
                    hasComeFromLevelCompletion = _levelProvider.CanChangeToNextLevel(true);
                }
                else
                {
                    persistentDataManager.SetPlayerDied();
                    
                    RunTracker runTracker = persistentDataManager.PlayerStats.RunTracker;
                    if (currentLevel.LevelContext.IsTutorial && runTracker.Deaths >= PlayerFirsts.TutorialDeathsUntilHowToPlaySuggested)
                    {
                        StartCoroutine(TryShowHowToPlayPopUpAfterDelay());
                    }
                }
            }
            
            _interLevelFlow.ShowInterLevelUI(new InterLevelFlow.InterLevelFlowSetupData
            {
                OnShown = ClearCurrentLevel,
                NewBadgeData = updatedStatisticsData?.NewBadgeData ?? new BadgeData(),
                NewFastestTimeInfo = updatedStatisticsData?.NewFastestTimeInfo,
                HasComeFromLevelCompletion = hasComeFromLevelCompletion,
                FirstTimeCompletingLevel = updatedStatisticsData?.FirstTimeCompletingLevel ?? false,
                LevelGotPerfect = completedPerfectLevel,
                AddedScore = updatedStatisticsData?.AddedToScore ?? -1,
                ShowingTutorial = _levelProvider.GetCurrentLevel().LevelContext.IsTutorial
            });
        }

        private IEnumerator CheckIfBeatGlobalRecordAfterDelay(string levelName)
        {
            if (PersistentDataManager.Instance.Options.DisableLeaderboardUpdates)
            {
                yield break;
            }
            
            yield return new WaitForSeconds(CheckGlobalLevelRankingDelay);
            
#if UNITY_EDITOR
            if (_dontUpdateLevelRankingsInEditor)
            {
                yield break;
            }
#endif
            
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName = "playerUpdateWorldBestLevels",
                FunctionParameter = new { LevelId = levelName }
            }, result =>
            {
                result.Logs.ApplyFunction(p => CircumDebug.Log(p.Message));
                bool valueChanged = (bool)result.FunctionResult;
                if (valueChanged)
                {
                    // trigger congrats
                    NewGlobalRecordPanel.ShowNewGlobalRecordFeedback();
                }
            }, error =>
            {
                CircumDebug.LogError($"Error checking whether player attained global ranking : {error}");
            });
        }

        private IEnumerator TryShowHowToPlayPopUpAfterDelay()
        {
            yield return new WaitForSeconds(ShowHowToPlayPopUpDelay);
            
            if (Settings.Instance.SettingsAreShowing)
            {
                // they might be finding how to play on their own at this point
                // if they die again then this will show then
                yield break;
            }
            PersistentDataManager.Instance.PlayerFirsts.ShowHowToPlayPopUpIfFirst();
        }
    }
}