using UnityEngine;

namespace Code.Level
{
    public class LevelContainer : MonoBehaviour
    {
        [SerializeField] private LevelLayout _levelLayout;

        public LevelLayout LevelLayout => _levelLayout;
    }
}