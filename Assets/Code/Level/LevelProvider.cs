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
        private int _levelIndex = 0;

        private int MaximumLevelIndex => _activeLevelProgression.TutorialLevelLayout.Length + _activeLevelProgression.LevelLayout.Length - 1;
        private int NumberOfTutorials => _activeLevelProgression.TutorialLevelLayout.Length;
        private int NumberOfLevels => _activeLevelProgression.LevelLayout.Length;
        private bool HasCompletedTutorials => _levelIndex >= _activeLevelProgression.TutorialLevelLayout.Length;
        
        public LevelProgression ActiveLevelProgression =>
            _activeLevelProgression
                ? _activeLevelProgression
                : _activeLevelProgression = Application.isEditor ? _editorLevelProgression : _platformLevelProgression;

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
                _levelIndex = NumberOfTutorials + restartLevelIndex;
                CircumDebug.Log($"Initialised level provider. Tutorials completed - set level index to {_levelIndex}");
            }
            else
            {
                CircumDebug.Log("Initialised level provider. Tutorials not complete");
            }
        }

        public LevelLayout GetCurrentLevel()
        {
            return GetLevelAtIndex(_levelIndex);
        }
        
        public void ResetToStart()
        {
            if (!HasCompletedTutorials)
            {
                _levelIndex = 0;
                return;
            }
            
            _levelIndex = NumberOfTutorials;
        }

        public void AdvanceLevel()
        {
            _levelIndex++;
            if (_levelIndex > MaximumLevelIndex)
            {
                _levelIndex = NumberOfTutorials;
            }
        }

        public void PreviousLevel()
        {
            _levelIndex--;
            if (_levelIndex < 0)
            {
                _levelIndex = _activeLevelProgression.TutorialLevelLayout.Length + _activeLevelProgression.LevelLayout.Length - 1;
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

        public void Validate()
        {
            _editorLevelProgression.Validate();
            _platformLevelProgression.Validate();
        }
    }
}