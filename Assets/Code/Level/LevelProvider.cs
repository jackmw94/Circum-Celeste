using Code.Debugging;
using Code.Level.Player;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    [DefaultExecutionOrder(-2)]
    public class LevelProvider : MonoBehaviour, IValidateable
    {
        [SerializeField] private LevelProgression _platformLevelProgression;
        [SerializeField] private LevelProgression _editorLevelProgression;
        
        private LevelProgression _activeLevelProgression;
        
        // internal index accounts for tutorials as well as regular levels. not the same index as in level context
        private int _internalLevelIndex = 0;

        private int MaximumLevelIndex => _activeLevelProgression.TutorialLevelLayout.Length + _activeLevelProgression.LevelLayout.Length - 1;
        private int NumberOfTutorials => _activeLevelProgression.TutorialLevelLayout.Length;
        private int NumberOfLevels => _activeLevelProgression.LevelLayout.Length;
        private bool HasCompletedTutorials => _internalLevelIndex >= _activeLevelProgression.TutorialLevelLayout.Length;
        
        public LevelProgression ActiveLevelProgression =>
            _activeLevelProgression
                ? _activeLevelProgression
                : _activeLevelProgression = Application.isEditor ? _editorLevelProgression : _platformLevelProgression;

        public LevelLayout GetCurrentLevel() => GetLevelAtIndex(_internalLevelIndex);

        public void Awake()
        {
            ActiveLevelProgression.Initialise();
        }

        private void Start()
        {
            PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
            _internalLevelIndex = persistentDataManager.GetRestartLevelIndex();

            if (persistentDataManager.PlayerStats.CompletedTutorials)
            {
                _internalLevelIndex = Mathf.Max(_internalLevelIndex, NumberOfTutorials);
            }
            
            CircumDebug.Log($"Initialised level provider. {(persistentDataManager.PlayerStats.CompletedTutorials ? "has" : "hasn't")} completed tutorials. internal level index = {_internalLevelIndex}");
        }

        public void ResetToStart(bool forceStartAtTutorials = false)
        {
            if (!HasCompletedTutorials || forceStartAtTutorials)
            {
                _internalLevelIndex = 0;
                return;
            }

            _internalLevelIndex = NumberOfTutorials;
            PersistentDataManager.Instance.SetCurrentLevel(_internalLevelIndex);
        }

        public void AdvanceLevel()
        {
            _internalLevelIndex = _internalLevelIndex == MaximumLevelIndex ? _internalLevelIndex : _internalLevelIndex + 1;
            PersistentDataManager.Instance.SetCurrentLevel(_internalLevelIndex);
        }

        public void PreviousLevel()
        {
            _internalLevelIndex = _internalLevelIndex == 0 ? 0 : _internalLevelIndex - 1;
            PersistentDataManager.Instance.SetCurrentLevel(_internalLevelIndex);
        }

        private LevelLayout GetLevelAtIndex(int index)
        {
            if (index < NumberOfTutorials)
            {
                return _activeLevelProgression.TutorialLevelLayout[index];
            }

            int levelIndex = Utilities.Mod(index - NumberOfTutorials, NumberOfLevels);
            return _activeLevelProgression.LevelLayout[levelIndex];
        }
        
        public bool CanChangeToNextLevel(bool allowTutorialAdvancing)
        {
            LevelLayoutContext levelContext = GetCurrentLevel().LevelContext;

            if (levelContext.IsTutorial && !allowTutorialAdvancing)
            {
                return false;
            }
            
            int currentLevelIndex = levelContext.LevelIndex;
            PlayerStats playerStats = PersistentDataManager.Instance.PlayerStats;
            bool nextLevelUnlocked = playerStats.IsNextLevelUnlocked(currentLevelIndex);
            
            return _internalLevelIndex < MaximumLevelIndex && nextLevelUnlocked;
        }

        public bool CanChangeToPreviousLevel()
        {
            LevelLayoutContext levelContext = GetCurrentLevel().LevelContext;

            if (levelContext.IsTutorial)
            {
                return false;
            }
            
            return _internalLevelIndex > NumberOfTutorials;
        }

        public void Validate()
        {
            _editorLevelProgression.Validate();
            _platformLevelProgression.Validate();
        }
    }
}