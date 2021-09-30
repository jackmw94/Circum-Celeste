using System.Collections;
using System.Collections.Generic;
using Code.Debugging;
using Code.Flow;
using Code.Level.Player;
using Code.UI;
using UnityEngine;
using UnityEngine.Analytics;

namespace Code.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private InterLevelFlow _interLevelFlow;
        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private PlayerContainer _playerContainer;
        [Space(15)]
        [SerializeField] private LevelGenerator _levelGenerator;
        
        private Coroutine _startLevelOnceMovedCoroutine = null;
        
        public LevelInstanceBase CurrentLevelInstance { get; private set; }

        public void CreateCurrentLevel(LevelRecording replay = null, InterLevelFlow.InterLevelTransition transition = InterLevelFlow.InterLevelTransition.Regular)
        {
            if (replay != null)
            {
                LevelLayoutContext levelContext = _levelProvider.GetCurrentLevel().LevelContext;
                Analytics.CustomEvent("StartedLevel", new Dictionary<string, object>
                {
                    { "LevelNumber", levelContext.LevelNumber },
                });
            }
            
            if (CurrentLevelInstance)
            {
                Destroy(CurrentLevelInstance);
            }
            
            if (!_interLevelFlow.IsOverlaid)
            {
                _interLevelFlow.ShowHideUI(() =>
                {
                    CreateLevelInternal(replay);
                }, transition);
            }
            else
            {
                CreateLevelInternal(replay);
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

            _interLevelFlow.ShowInterLevelUI(ClearCurrentLevel, InterLevelFlow.InterLevelTransition.Fast);
        }

        private void ClearCurrentLevel()
        {
            _levelGenerator.DestroyLevel();
        }

        private void CreateLevelInternal(LevelRecording replay)
        {
            LevelLayout levelLayout = _levelProvider.GetCurrentLevel();
            
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
            _startLevelOnceMovedCoroutine = StartCoroutine(StartLevelWhenReady());
        }
        
        private IEnumerator StartLevelWhenReady()
        {
            _interLevelFlow.HideInterLevelUI();
            yield return new WaitUntil(() => !_interLevelFlow.IsOverlaid);
            
            CurrentLevelInstance.LevelReady();
            
            yield return new WaitUntil(() => CurrentLevelInstance.PlayerStartedPlaying);

            CircumDebug.Log($"Level '{CurrentLevelInstance.name}' has started");
            CurrentLevelInstance.StartLevel(OnLevelFinished);
        }

        private void OnLevelFinished(LevelResult levelResult)
        {
            bool isReplay = levelResult.WasReplay;
            BadgeData newBadgeData = new BadgeData();
            NewFastestTimeInfo newFastestTimeInfo = null;
            bool advanceLevelPrompt = false;

            if (!isReplay)
            {
                PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
                LevelLayout currentLevel = _levelProvider.GetCurrentLevel();
                
                if (levelResult.Success)
                {
                    LevelLayoutContext levelContext = currentLevel.LevelContext;

                    LevelRecording levelRecording = new LevelRecording
                    {
                        LevelIndex = levelContext.LevelIndex,
                        RecordingData = levelResult.LevelRecordingData
                    };
                    
                    Analytics.CustomEvent("CompletedLevel", new Dictionary<string, object>
                    {
                        { "LevelNumber", levelContext.LevelNumber },
                        { "NoDamage", levelResult.NoDamage },
                        { "GoldTime", levelRecording.HasBeatenGoldTime(currentLevel.GoldTime) },
                    });

                    persistentDataManager.UpdateStatisticsAfterLevel(currentLevel, levelResult.NoDamage, levelRecording, out newBadgeData, out newFastestTimeInfo);

                    advanceLevelPrompt = _levelProvider.CanChangeToNextLevel(true);
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
            
            _interLevelFlow.ShowInterLevelUI(ClearCurrentLevel, newBadgeData: newBadgeData, newFastestTimeInfo: newFastestTimeInfo, showAdvanceLevelPrompt: advanceLevelPrompt);
        }

        private IEnumerator TryShowHowToPlayPopUpAfterDelay()
        {
            yield return new WaitForSeconds(1.33f);
            
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