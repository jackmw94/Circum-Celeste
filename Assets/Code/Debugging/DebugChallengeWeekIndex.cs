using System;
using Code.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Debugging
{
    public class DebugChallengeWeekIndex : MonoBehaviour
    {
        [SerializeField] private Button _previousWeekButton;
        [SerializeField] private Button _nextWeekButton;
        [SerializeField] private Button _resetWeekButton;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _label;

        private ChallengeScreen _challengeScreen;
        private bool _isDirty = false;

#if CIRCUM_DEBUG
        
        private void Awake()
        {
            _challengeScreen = UnityCommonFeatures.UIPanel.GetPanel<ChallengeScreen>();
            _previousWeekButton.onClick.AddListener(PreviousWeekListener);
            _nextWeekButton.onClick.AddListener(NextWeekListener);
            _resetWeekButton.onClick.AddListener(ResetDebugWeekListener);
        }

        private void OnEnable()
        {
            _isDirty = false;
            SetCurrentWeekIndexLabel();
        }

        private void OnDisable()
        {
            _challengeScreen.SetupChallengeScreen();
        }

        private void OnDestroy()
        {
            _previousWeekButton.onClick.RemoveListener(PreviousWeekListener);
            _nextWeekButton.onClick.RemoveListener(NextWeekListener);
            _resetWeekButton.onClick.RemoveListener(ResetDebugWeekListener);
        }

        private void PreviousWeekListener()
        {
            _isDirty = true;
            _challengeScreen.SetDebugChallengeWeek(true, false, -1);
            SetCurrentWeekIndexLabel();
        }
        
        private void NextWeekListener()
        {
            _isDirty = true;
            _challengeScreen.SetDebugChallengeWeek(true, false, 1);
            SetCurrentWeekIndexLabel();
        }
        
        private void ResetDebugWeekListener()
        {
            _isDirty = true;
            _challengeScreen.SetDebugChallengeWeek(false, true);
            SetCurrentWeekIndexLabel();
        }

        private void SetCurrentWeekIndexLabel()
        {
            _label.text = $"Current week index: {ChallengeScreen.WeekIndex.ToString()}";
        }
        
#endif
    }
}