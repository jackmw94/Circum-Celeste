using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelProvider : MonoBehaviour, IValidateable
    {
        [SerializeField] private LevelProgression _platformLevelProgression;
        [SerializeField] private LevelProgression _editorLevelProgression;

        private LevelProgression _activeLevelProgression;
        private int _levelIndex = 0;

        private int NumberOfTutorials => _activeLevelProgression.TutorialLevelLayout.Length;
        private int NumberOfLevels => _activeLevelProgression.LevelLayout.Length;
        private bool HasCompletedTutorials => _levelIndex >= _activeLevelProgression.TutorialLevelLayout.Length;

        public void Initialise(bool hasCompletedTutorials, int lastLevelPlayed)
        {
            _activeLevelProgression = Application.isEditor ? _editorLevelProgression : _platformLevelProgression;
            _activeLevelProgression.Initialise();
            
            if (hasCompletedTutorials)
            {
                int lastLevelIndex = lastLevelPlayed - 1;
                _levelIndex = NumberOfTutorials + lastLevelIndex;
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
        
        public LevelLayout GetNextLevel()
        {
            return GetLevelAtIndex(_levelIndex + 1);
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