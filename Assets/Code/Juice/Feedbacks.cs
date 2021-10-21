using System;
using System.Collections.Generic;
using System.Text;
using Code.Debugging;
using Code.Level;
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

        public enum VibrationType
        {
            None,
            Regular,
            Weak,
            Strong,
            Nope
        }

        [Serializable]
        public class FeedbackSetting
        {
            [SerializeField] private FeedbackType _feedbackType;
            [SerializeField, Range(0f, 1f)] private float _screenShakeAmount;
            [SerializeField] private VibrationType _vibrationType;
            [SerializeField] private TimeControl.TimeControlFeedback _timeControlFeedback;
            [SerializeField] private bool _triggerVignette;
            
            public FeedbackType FeedbackType => _feedbackType;
            public float ScreenShakeAmount => _screenShakeAmount;
            public VibrationType VibrationType => _vibrationType;
            public TimeControl.TimeControlFeedback TimeControlFeedback => _timeControlFeedback;
            public bool TriggerVignette => _triggerVignette;
        }
        
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
            
            Vibration.Init();
            
            _activeState.SetUnsetReason(ActiveState.ActiveReason.UserSetting, true);
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

            TriggerVibration(feedbackSetting.VibrationType);
            _screenShake.AddShake(feedbackSetting.ScreenShakeAmount);
            _timeControl.AddTimeState(feedbackSetting.TimeControlFeedback);
            if (feedbackSetting.TriggerVignette)
            {
                _vignetteFeedback.TriggerAnimation();
            }
        }

        private void TriggerVibration(VibrationType vibrationType)
        {
            switch (vibrationType)
            {
                case VibrationType.None: return;
                case VibrationType.Regular: Vibration.Vibrate(); break;
                case VibrationType.Weak: Vibration.VibratePop(); break;
                case VibrationType.Strong: Vibration.VibratePeek(); break;
                case VibrationType.Nope: Vibration.VibrateNope(); break;
                default: throw new ArgumentOutOfRangeException(nameof(vibrationType), vibrationType, null);
            }
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