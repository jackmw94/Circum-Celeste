using System.Linq;
using UnityEngine;
using UnityExtras.Core;

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
                // first tutorial is -1, second -2, etc.
                LevelLayoutContext tutorialLevelContext = _tutorialLevels[i].LevelContext;
                tutorialLevelContext.LevelIndex = -i - 1;
                tutorialLevelContext.IsFinalTutorial = i == _tutorialLevels.Length - 1;
                tutorialLevelContext.ContributesToScoring = false;
            }

            for (int i = 0; i < _levelLayout.Length; i++)
            {
                LevelLayoutContext levelContext = _levelLayout[i].LevelContext;
                levelContext.LevelIndex = i;
                levelContext.ContributesToScoring = _levelLayout[i].ContributesToScoring;
            }
        }
        
        public void Validate()
        {
            _levelLayout.ApplyFunction(p => p.Validate());
            
            int distinctIds = _levelLayout.Select(p => p.LevelId).Distinct().Count();
            Debug.Assert(distinctIds == _levelLayout.Length, $"There are duplicate level ids! It is highly likely there has been a bug:\n{_levelLayout.OrderBy(p => p.LevelId).Select(p => $"{p.name}:{p.LevelId}").JoinToString("\n")}");
        }
    }
}