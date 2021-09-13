using System;
using Code.Core;
using Code.Level.Player;
using UnityEngine;

namespace Code.Level
{
    public abstract class LevelInstanceBase : MonoBehaviour
    {
        private Action<LevelResult> _levelFinishedCallback;
        private LevelTimeUI _levelTimeUI;
        
        public virtual bool PlayerStartedPlaying => true;
        protected bool IsStarted { get; private set; }
        private CircumOptions PlayerOptions => PersistentDataManager.Instance.Options;

        private void Awake()
        {
            _levelTimeUI = GameContainer.Instance.LevelTimeUI;
            _levelTimeUI.SettingsShowHideTime(PlayerOptions.ShowLevelTimer);
        }

        private void OnDestroy()
        {
            _levelTimeUI.StartStopTimer(false);
            _levelTimeUI.GameplayShowHideTime(false);
        }

        public void LevelReady()
        {
            _levelTimeUI.ResetTimer();
            _levelTimeUI.GameplayShowHideTime(true);
            OnLevelReady();
        }

        public void StartLevel(Action<LevelResult> levelFinishedCallback)
        {
            IsStarted = true;
            _levelFinishedCallback = levelFinishedCallback;
            _levelTimeUI.StartStopTimer(true);
            OnStartLevel();
        }

        protected void LevelFinished(LevelResult levelResult)
        {
            IsStarted = false;
            _levelFinishedCallback(levelResult);
            
            _levelTimeUI.ManuallySetTimer(levelResult.LevelRecordingData.LevelTime);
            _levelTimeUI.StartStopTimer(false);
        }

        protected abstract void OnLevelReady();

        protected abstract void OnStartLevel();

        public abstract Vector3 GetPlayerPosition(int playerIndex);
    }
}