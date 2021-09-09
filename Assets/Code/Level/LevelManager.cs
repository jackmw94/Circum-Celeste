using System.Collections;
using Code.Debugging;
using Code.Flow;
using Code.Level.Player;
using UnityEngine;

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
        
        public LevelInstanceBase CurrentLevel { get; private set; }

        public void CreateCurrentLevel(LevelRecording replay = null, InterLevelFlow.InterLevelTransition transition = InterLevelFlow.InterLevelTransition.Regular)
        {
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
            if (!_levelProvider.GetCurrentLevel().LevelContext.IsFirstLevel)
            {
                PersistentDataManager.Instance.SetPlayerDied();
            }

            if (_startLevelOnceMovedCoroutine != null)
            {
                StopCoroutine(_startLevelOnceMovedCoroutine);
            }

            _interLevelFlow.ShowInterLevelUI(ClearCurrentLevel);
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
                CurrentLevel = _levelGenerator.GenerateReplay(replay.RecordingData, levelLayout);
            }
            else
            {
                InputProvider[] inputProviders = _playerContainer.GetPlayerInputProviders();
                CurrentLevel = _levelGenerator.GenerateLevel(inputProviders, levelLayout);
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
            
            CurrentLevel.LevelReady();
            
            yield return new WaitUntil(() => CurrentLevel.PlayerStartedPlaying);

            CircumDebug.Log($"Level '{CurrentLevel.name}' has started");
            CurrentLevel.StartLevel(OnLevelFinished);
        }

        private void OnLevelFinished(LevelResult levelResult)
        {
            bool isReplay = levelResult.WasReplay;
            BadgeData newBadgeData = new BadgeData();
            bool advanceLevelPrompt = false;
            
            if (!isReplay && levelResult.Success)
            {
                LevelLayout currentLevel = _levelProvider.GetCurrentLevel();
                LevelLayoutContext levelContext = currentLevel.LevelContext;
                
                LevelRecording levelRecording = new LevelRecording
                {
                    LevelIndex = levelContext.LevelIndex,
                    RecordingData = levelResult.LevelRecordingData
                };
                PersistentDataManager.Instance.UpdateStatisticsAfterLevel(currentLevel, levelResult.NoDamage, levelRecording, out newBadgeData);

                advanceLevelPrompt = true;
                //_levelProvider.AdvanceLevel();
            }
            
            _interLevelFlow.ShowInterLevelUI(ClearCurrentLevel, newBadgeData: newBadgeData, showAdvanceLevelPrompt: advanceLevelPrompt);
        }
    }
}