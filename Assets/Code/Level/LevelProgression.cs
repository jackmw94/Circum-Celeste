using System.Linq;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create LevelFlow", fileName = "LevelFlow", order = 0)]
    public class LevelProgression : ScriptableObject
    {
        [SerializeField] private LevelLayout[] _tutorialLevels;
        [SerializeField] private LevelLayout[] _levelLayout;

        public LevelLayout[] TutorialLevelLayout => _tutorialLevels;
        public LevelLayout[] LevelLayout => _levelLayout;
        
        public void Initialise()
        {
            for (int i = 0; i < _tutorialLevels.Length; i++)
            {
                _tutorialLevels[i].LevelNumber = 0;
            }

            for (int i = 0; i < _levelLayout.Length; i++)
            {
                // user facing level number, rather than index
                _levelLayout[i].LevelNumber = i + 1;
            }
        }
        
        public void Validate()
        {
            _levelLayout.ApplyFunction(p => p.Validate());
        }
    }
}