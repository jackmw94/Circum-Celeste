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
            int restartLevelIndex = persistentDataManager.GetRestartLevelIndex();
            bool hasCompletedTutorials = persistentDataManager.PlayerStats.CompletedTutorials;

            if (hasCompletedTutorials)
            {
                _internalLevelIndex = NumberOfTutorials + restartLevelIndex;
                CircumDebug.Log($"Initialised level provider. Tutorials completed - set level index to {_internalLevelIndex}");
            }
            else
            {
                CircumDebug.Log("Initialised level provider. Tutorials not complete");
            }
        }

        public void ResetToStart(bool forceStartAtTutorials = false)
        {
            if (!HasCompletedTutorials || forceStartAtTutorials)
            {
                _internalLevelIndex = 0;
                return;
            }

            _internalLevelIndex = NumberOfTutorials;
        }

        public void AdvanceLevel()
        {
            _internalLevelIndex++;
            if (_internalLevelIndex > MaximumLevelIndex)
            {
                _internalLevelIndex = NumberOfTutorials;
            }
        }

        public void PreviousLevel()
        {
            _internalLevelIndex--;
            if (_internalLevelIndex < 0)
            {
                _internalLevelIndex = _activeLevelProgression.TutorialLevelLayout.Length + _activeLevelProgression.LevelLayout.Length - 1;
            }
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
        
        public bool CanChangeToNextLevel()
        {
            int currentLevelIndex = GetCurrentLevel().LevelContext.LevelIndex;
            PlayerStats playerStats = PersistentDataManager.Instance.PlayerStats;
            bool nextLevelUnlocked = playerStats.IsNextLevelUnlocked(currentLevelIndex);
            
            return _internalLevelIndex < MaximumLevelIndex && nextLevelUnlocked;
        }

        public bool CanChangeToPreviousLevel()
        {
            return _internalLevelIndex > NumberOfTutorials;
        }

        public void Validate()
        {
            _editorLevelProgression.Validate();
            _platformLevelProgression.Validate();
        }
    }
}