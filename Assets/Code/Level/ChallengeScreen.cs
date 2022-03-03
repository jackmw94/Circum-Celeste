using System;
using System.Collections;
using System.Linq;
using Code.Core;
using Code.Debugging;
using Code.Level.Player;
using Code.UI;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UIPanel = UnityCommonFeatures.UIPanel;

namespace Code.Level
{
    public class ChallengeScreen : UIPanel
    {
        private const float CheckForNextWeekIndexDelay = 120f;
        private const float UpdateScreenOnLevelStartDelay = 1.5f;
        private static readonly DateTime DayZero = new DateTime(2022, 2, 14, 0, 0, 0);

        [SerializeField] private LevelManager _levelManager;
        [Space(15)] 
        [SerializeField] private TextMeshProUGUI _levelNameLabel;
        [SerializeField] private TextMeshProUGUI _daysRemainingLabel;
        [SerializeField] private TextMeshProUGUI _attemptsRemainingLabel;
        [SerializeField] private TextMeshProUGUI _scoreLabel;
        [SerializeField] private TextMeshProUGUI _playButtonLabel;
        [SerializeField] private Button _playButton;
        [Space(15)]
        [SerializeField, LeanTranslationName] private string _challengeScoreTranslation;
        [SerializeField, LeanTranslationName] private string _daysRemainingTranslation;
        [SerializeField, LeanTranslationName] private string _attemptsRemainingTranslation;
        [Space(15)]
        [SerializeField] private Color _playButtonDefaultTextColour;
        [SerializeField] private Color _playButtonNoAttemptsColour;
        [SerializeField] private PulseTextColour _pulseTextColour;
        [Space(30)]
        [SerializeField] private bool _useDebugWeekIndex = false;
        [SerializeField] private int _debugWeekIndex = 2;

        private bool _anyAttemptsRemaining = false;
        private ChallengeLevel _currentChallengeLevel = null;
        private int _cachedWeekIndex;
        
        private static int CurrentDayIndex => (DateTime.Now - DayZero).Days;
        public static int WeekIndex
        {
            get
            {
#if CIRCUM_DEBUG
                ChallengeScreen challengeScreen = GetPanel<ChallengeScreen>();
                return challengeScreen._useDebugWeekIndex ? challengeScreen._debugWeekIndex : CurrentWeekIndexInternal;
#else
                return CurrentWeekIndexInternal;
#endif
            }
        }
        private static int CurrentWeekIndexInternal => CurrentDayIndex / 7;

        protected override void InternalAwake()
        {
            base.InternalAwake();
            _playButton.onClick.AddListener(PlayLevel);
        }

        protected void OnEnable()
        {
            StartCoroutine(CheckForChallengeWeekIncremented());
        }

        protected override void InternalOnDestroy()
        {
            base.InternalOnDestroy();
            _playButton.onClick.RemoveListener(PlayLevel);
        }

        protected override void InternalStart()
        {
            base.InternalStart();
            StartCoroutine(SetupOnceLoggedIn());
        }

        private IEnumerator SetupOnceLoggedIn()
        {
            yield return new WaitUntil(() => RemoteDataManager.Instance.IsLoggedIn);
            SetupChallengeScreen();
        }

        [ContextMenu(nameof(SetupChallengeScreen))]
        public void SetupChallengeScreen()
        {
            ChallengeData challengeData = PersistentDataManager.Instance.ChallengeData;
            bool attemptDataIsCurrent = challengeData.AttemptData.DayIndex == CurrentDayIndex;

            if (!attemptDataIsCurrent)
            {
                challengeData.AttemptData = new ChallengeData.ChallengeAttemptData
                {
                    DayIndex = CurrentDayIndex,
                    AttemptCount = 0
                };
            }

            _playButton.interactable = false;
            _cachedWeekIndex = WeekIndex;
            
            ChallengeLevel.RequestChallengeLevel(WeekIndex, challengeLevel => { SetupChallengeScreenInternal(challengeLevel, challengeData); });
        }

        private void SetupChallengeScreenInternal(ChallengeLevel challengeLevel, ChallengeData challengeData)
        {
            _currentChallengeLevel = challengeLevel;
            _levelNameLabel.text = challengeLevel.LevelName;

            int attemptsRemaining = challengeLevel.AttemptsRemaining(challengeData.AttemptData.AttemptCount);
            string attemptsRemainingTranslation = LeanLocalization.GetTranslationText(_attemptsRemainingTranslation);
            _attemptsRemainingLabel.text = string.Format(attemptsRemainingTranslation, attemptsRemaining);

            int score = challengeData.ChallengeScores.FirstOrDefault(p => p.WeekIndex == WeekIndex)?.Score ?? 0;
            string scoreTranslation = LeanLocalization.GetTranslationText(_challengeScoreTranslation);
            _scoreLabel.text = string.Format(scoreTranslation, score, _currentChallengeLevel.Points);

            int daysRemaining = 6 - CurrentDayIndex + WeekIndex * 7;
            string daysRemainingTranslation = LeanLocalization.GetTranslationText(_daysRemainingTranslation);
            _daysRemainingLabel.text = string.Format(daysRemainingTranslation, daysRemaining);

            _anyAttemptsRemaining = attemptsRemaining > 0;
            _playButton.interactable = true;
            _playButtonLabel.color = _anyAttemptsRemaining ? _playButtonDefaultTextColour : _playButtonNoAttemptsColour;
        }

        private void PlayLevel()
        {
            if (!_anyAttemptsRemaining)
            {
                _pulseTextColour.PulseColour();
                return;
            }
            
            PersistentDataManager.Instance.ChallengeData.ChallengeAttempted();
            _levelManager.CreateChallengeLevel(_currentChallengeLevel, OnChallengeCompleted);
            StartCoroutine(UpdateScreenAfterDelay(UpdateScreenOnLevelStartDelay));
        }

        private IEnumerator UpdateScreenAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetupChallengeScreenInternal(_currentChallengeLevel, PersistentDataManager.Instance.ChallengeData);
        }

        private void OnChallengeCompleted(LevelResult levelResult)
        {
            ChallengeData challengeData = PersistentDataManager.Instance.ChallengeData;
            if (levelResult.Success)
            {
                float fullMarksTime = _currentChallengeLevel.LevelLayout.FullMarksTime;
                float recordingTime = levelResult.LevelRecordingData.LevelTime;
                bool isPerfect = levelResult.LevelRecordingData.IsPerfect;
                int levelScore = PlayerScoreHelper.GetScoreFromLevel(fullMarksTime, recordingTime, isPerfect, _currentChallengeLevel.Points);
                challengeData.ChallengeScored(_currentChallengeLevel.WeekIndex, levelScore);
            }
            
            _levelManager.ExitLevel();
            SetupChallengeScreenInternal(_currentChallengeLevel, challengeData);
        }

        private IEnumerator CheckForChallengeWeekIncremented()
        {
            while (true)
            {
                yield return new WaitForSeconds(CheckForNextWeekIndexDelay);
                if (_cachedWeekIndex != WeekIndex)
                {
                    CircumDebug.Log($"Found the current week index is not the same as the challenge we loaded last. Refreshing. ({WeekIndex} != {_cachedWeekIndex})");
                    SetupChallengeScreen();
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
#if CIRCUM_DEBUG
        public void SetDebugChallengeWeek(bool useDebugWeekIndex, bool setDebugWeekIndexToCurrent, int weekIndexOffset = 0)
        {
            _useDebugWeekIndex = useDebugWeekIndex;
            if (setDebugWeekIndexToCurrent)
            {
                _debugWeekIndex = CurrentWeekIndexInternal;
            }
            else
            {
                _debugWeekIndex += weekIndexOffset;
            }
        }
#endif
    }
}