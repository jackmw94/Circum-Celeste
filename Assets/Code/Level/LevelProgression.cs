using UnityEngine;

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create LevelFlow", fileName = "LevelFlow", order = 0)]
    public class LevelProgression : ScriptableObject
    {
        [SerializeField] private LevelLayout[] _levelLayout;
        
        public LevelLayout[] LevelLayout => _levelLayout;
    }
}