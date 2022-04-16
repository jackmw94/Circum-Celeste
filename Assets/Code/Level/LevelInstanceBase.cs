using System;
using Code.Core;
using Code.Juice;
using Code.Level.Player;
using UnityEngine;

namespace Code.Level
{
    public abstract class LevelInstanceBase : MonoBehaviour
    {
        private Action<LevelResult> _levelFinishedCallback;
        private LevelTimeUI _levelTimeUI;
        private Feedbacks _feedbacks;
        
        public virtual bool PlayerStartedPlaying => true;
        protected bool IsStarted { get; private set; }
        private bool _hasFinished = false;

        public static Action<bool> LevelCreated = (isChallenge) => { };
        public static Action LevelStarted = () => { };
        public static Action LevelStopped = () => { };
        
        private CircumOptions PlayerOptions => PersistentDataManager.Instance.Options;

        protected virtual void Awake()
        {
            _feedbacks = Feedbacks.Instance;
            
            _levelTimeUI = GameContainer.Instance.LevelTimeUI;
            _levelTimeUI.SettingsShowHideTime(PlayerOptions.ShowLevelTimer);
        }
        
        protected virtual void OnDestroy()
        {
            if (!_hasFinished)
            {
                LevelStopped();
            }
            
            _levelTimeUI.StartStopTimer(false);
            _levelTimeUI.GameplayShowHideTime(false);
        }

        public void LevelReady()
        {
            _feedbacks.SetActiveInactive(ActiveState.ActiveReason.Gameplay, true);
            
            _levelTimeUI.ResetTimer();
            _levelTimeUI.GameplayShowHideTime(true);
            OnLevelReady();
        }

        public void StartLevel(Action<LevelResult> levelFinishedCallback)
        {
            IsStarted = true;
            LevelStarted();
            
            _levelFinishedCallback = levelFinishedCallback;
            _levelTimeUI.StartStopTimer(true);
            OnStartLevel();
        }

        protected void LevelFinished(LevelResult levelResult)
        {
            IsStarted = false;
            _hasFinished = true;
            LevelStopped();
            _levelFinishedCallback(levelResult);
            
            _feedbacks.SetActiveInactive(ActiveState.ActiveReason.Gameplay, false);
            
            _levelTimeUI.ManuallySetTimer(levelResult.LevelRecordingData.LevelTime);
            _levelTimeUI.StartStopTimer(false);
        }

        protected abstract void OnLevelReady();

        protected abstract void OnStartLevel();

        public abstract Vector3 GetPlayerPosition(int playerIndex);
    }
}