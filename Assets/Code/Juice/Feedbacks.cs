using System;
using System.Collections.Generic;
using Code.Core;
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
            PlayerDamaged
        }

        [Serializable]
        public class FeedbackSetting
        {
            [SerializeField] private FeedbackType _feedbackType;
            [SerializeField, Range(0f, 1f)] private float _screenShakeAmount;
            [SerializeField] private float _vibrationDuration;
            [SerializeField] private TimeControl.TimeControlFeedback _timeControlFeedback;
            
            public FeedbackType FeedbackType => _feedbackType;
            public float ScreenShakeAmount => _screenShakeAmount;
            public float VibrationDuration => _vibrationDuration;
            public TimeControl.TimeControlFeedback TimeControlFeedback => _timeControlFeedback;
        }
        
        [SerializeField] private Vibration _vibration;
        [SerializeField] private TimeControl _timeControl;
        [SerializeField] private ScreenShake _screenShake;
        [Space(15)]
        [SerializeField] private FeedbackSetting[] _feedbackSettings;

        private readonly Dictionary<FeedbackType, FeedbackSetting> _feedbackSettingsByType = new Dictionary<FeedbackType, FeedbackSetting>();

        public bool FeedbacksActive { get; set; } = true;
        
        private void Awake()
        {
            RegenerateFeedbacksDictionary();
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

        public void TriggerFeedback(FeedbackType feedbackType)
        {
            if (!FeedbacksActive)
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
        }
    }
}