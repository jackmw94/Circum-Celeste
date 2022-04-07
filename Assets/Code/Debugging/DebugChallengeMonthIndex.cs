using System;
using Code.Level;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.Debugging
{
    public class DebugChallengeMonthIndex : MonoBehaviour
    {
        [FormerlySerializedAs("_previousWeekButton")] [SerializeField] private Button _previousMonthButton;
        [FormerlySerializedAs("_nextWeekButton")] [SerializeField] private Button _nextMonthButton;
        [FormerlySerializedAs("_resetWeekButton")] [SerializeField] private Button _resetMonthButton;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _label;

        private ChallengeScreen _challengeScreen;
        private bool _isDirty = false;

#if CIRCUM_DEBUG
        
        private void Awake()
        {
            _challengeScreen = UnityCommonFeatures.UIPanel.GetPanel<ChallengeScreen>();
            _previousMonthButton.onClick.AddListener(PreviousWeekListener);
            _nextMonthButton.onClick.AddListener(NextWeekListener);
            _resetMonthButton.onClick.AddListener(ResetDebugWeekListener);
        }

        private void OnEnable()
        {
            _isDirty = false;
            SetCurrentMonthIndexLabel();
        }

        private void OnDisable()
        {
            _challengeScreen.SetupChallengeScreen();
        }

        private void OnDestroy()
        {
            _previousMonthButton.onClick.RemoveListener(PreviousWeekListener);
            _nextMonthButton.onClick.RemoveListener(NextWeekListener);
            _resetMonthButton.onClick.RemoveListener(ResetDebugWeekListener);
        }

        private void PreviousWeekListener()
        {
            _isDirty = true;
            _challengeScreen.SetDebugChallengeMonth(true, false, -1);
            SetCurrentMonthIndexLabel();
        }
        
        private void NextWeekListener()
        {
            _isDirty = true;
            _challengeScreen.SetDebugChallengeMonth(true, false, 1);
            SetCurrentMonthIndexLabel();
        }
        
        private void ResetDebugWeekListener()
        {
            _isDirty = true;
            _challengeScreen.SetDebugChallengeMonth(false, true);
            SetCurrentMonthIndexLabel();
        }

        private void SetCurrentMonthIndexLabel()
        {
            _label.text = $"Current month index: {ChallengeScreen.MonthIndex.ToString()}";
        }
        
#endif
    }
}