using System;
using System.Collections.Generic;
using System.Text;
using Code.Core;
using Code.Debugging;
using Code.Level;
using Code.Level.Player;
using Code.VFX;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Juice
{
    public class Feedbacks : SingletonMonoBehaviour<Feedbacks>
    {
        public enum FeedbackType
        {
            HitEnemy,
            CollectedPickup,
            CompletedLevel,
            PlayerDamaged,
        }

        [Serializable]
        public class FeedbackSetting
        {
            [SerializeField] private FeedbackType _feedbackType;
            [SerializeField, Range(0f, 1f)] private float _screenShakeAmount;
            [SerializeField] private float _vibrationDuration;
            [SerializeField] private TimeControl.TimeControlFeedback _timeControlFeedback;
            [SerializeField] private bool _triggerVignette;
            
            public FeedbackType FeedbackType => _feedbackType;
            public float ScreenShakeAmount => _screenShakeAmount;
            public float VibrationDuration => _vibrationDuration;
            public TimeControl.TimeControlFeedback TimeControlFeedback => _timeControlFeedback;
            public bool TriggerVignette => _triggerVignette;
        }
        
        [SerializeField] private Vibration _vibration;
        [SerializeField] private TimeControl _timeControl;
        [SerializeField] private ScreenShaker _screenShake;
        [SerializeField] private AnimateVignette _vignetteFeedback;
        [Space(15)]
        [SerializeField] private FeedbackSetting[] _feedbackSettings;

        private readonly Dictionary<FeedbackType, FeedbackSetting> _feedbackSettingsByType = new Dictionary<FeedbackType, FeedbackSetting>();

        private ActiveState _activeState = new ActiveState();
        
        private void Awake()
        {
            RegenerateFeedbacksDictionary();
            RemoteConfigHelper.RemoteConfigUpdated += SetFeedbacksFromRemoteConfig;
            
            _activeState.SetUnsetReason(ActiveState.ActiveReason.UserSetting, true);
        }

        private void OnDestroy()
        {
            RemoteConfigHelper.RemoteConfigUpdated -= SetFeedbacksFromRemoteConfig;
        }

        [ContextMenu(nameof(RegenerateFeedbacksDictionary))]
        private void RegenerateFeedbacksDictionary()
        {
            _feedbackSettingsByType.Clear();
            foreach (FeedbackSetting feedbackSetting in _feedbackSettings)
            {
                _feedbackSettingsByType.Add(feedbackSetting.FeedbackType, feedbackSetting);
            }
        }

        public void SetActiveInactive(ActiveState.ActiveReason reason, bool setActive)
        {
            _activeState.SetUnsetReason(reason, setActive);
        }

        public bool IsActiveForReason(ActiveState.ActiveReason reason)
        {
            return _activeState.IsActiveForReason(reason);
        }

        public void TriggerFeedback(FeedbackType feedbackType)
        {
            if (!_activeState.IsActive)
            {
                return;
            }
            
            if (!_feedbackSettingsByType.TryGetValue(feedbackType, out FeedbackSetting feedbackSetting))
            {
                Debug.LogError($"There is no feedback type for {feedbackType}");
                return;
            }
            _vibration.AddVibration(feedbackSetting.VibrationDuration);
            _screenShake.AddShake(feedbackSetting.ScreenShakeAmount);
            _timeControl.AddTimeState(feedbackSetting.TimeControlFeedback);
            if (feedbackSetting.TriggerVignette)
            {
                _vignetteFeedback.TriggerAnimation();
            }
        }

        private void SetFeedbacksFromRemoteConfig()
        {
            if (string.IsNullOrEmpty(RemoteConfigHelper.FeedbackProperties))
            {
                return;
            }

            // bit of a hack - we use a single string for feedback settings with objects separated by #
            string[] splitSerializedFeedbackSettings = RemoteConfigHelper.FeedbackProperties.Split('#');
            FeedbackSetting[] feedbackSettings = new FeedbackSetting[splitSerializedFeedbackSettings.Length];
            for (int i = 0; i < feedbackSettings.Length; i++)
            {
                feedbackSettings[i] = JsonUtility.FromJson<FeedbackSetting>(splitSerializedFeedbackSettings[i]);
            }

            _feedbackSettings = feedbackSettings;
            RegenerateFeedbacksDictionary();
        }

        [ContextMenu(nameof(PrintSerializedFeedbacks))]
        private void PrintSerializedFeedbacks()
        {
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < _feedbackSettings.Length; index++)
            {
                FeedbackSetting feedbackSetting = _feedbackSettings[index];
                string serialized = JsonUtility.ToJson(feedbackSetting);
                sb.Append(serialized);
                if (index + 1 < _feedbackSettings.Length)
                {
                    sb.Append("#");
                }
            }
            
            CircumDebug.Log(sb.ToString());
        }

        private void SetFeedbacksOnRemoteConfig()
        {
            
        }
    }
}