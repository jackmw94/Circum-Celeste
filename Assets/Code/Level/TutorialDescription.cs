using System;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create TutorialDescription", fileName = "TutorialDescription", order = 0)]
    public class TutorialDescription : ScriptableObject
    {
        [Serializable]
        public class TutorialCommand
        {
            public enum TutorialCommandCompletionCriteria
            {
                Timer,
                IsMoving,
                AllPickupsCollected,
                EnteredEscape
            }
            
            public bool OrbiterEnabled;
            public bool OrbiterDamageEnabled;
            public bool PickupsEnabled;
            public bool HazardsEnabled;
            public bool BlackHolesEnabled;
            public bool EscapeEnabled;
            [LeanTranslationName, SerializeField, FormerlySerializedAs("Instruction")] private string _instruction;
            public IntroduceElement IntroduceElement;
            public TutorialCommandCompletionCriteria CompleteCommandCriteria;
            public float Duration;

            public string GetLocalisedInstruction()
            {
                return string.IsNullOrEmpty(_instruction) ? "" : LeanLocalization.GetTranslationText(_instruction, _instruction);
            }
        }

        [SerializeField] private TutorialCommand[] _tutorialCommands;
        
        public TutorialCommand[] TutorialCommands => _tutorialCommands;
    }
}