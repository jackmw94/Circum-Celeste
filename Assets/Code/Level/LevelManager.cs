using System.Collections;
using Code.Player;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelLayout _debugLevelLayout;
        [SerializeField] private LevelProgression _levelProgression;
        [Space(15)]
        [SerializeField] private User[] _users;
        [Space(15)]
        [SerializeField] private LevelGenerator _levelGenerator;
        
        private void Start()
        {
            // todo: move player code out of here:
            Debug.Assert(_users.Length > 0,"There are no users for this level, probably not what we want..");
            _users.ApplyFunction(p => p.Player.Initialise(5, p.InputProvider));
            
            CreateLevel();
        }

        private void CreateLevel()
        {
            // todo: replace this with progress / requested level
#if UNITY_EDITOR
            if (_debugLevelLayout)
            {
                GenerateDebugLevel();
                return;
            }
#endif

            GenerateLevel(_levelProgression.LevelLayout[0]);
        }

        [ContextMenu(nameof(GenerateDebugLevel))]
        private void GenerateDebugLevel()
        {
            GenerateLevel(_debugLevelLayout);
        }

        private void GenerateLevel(LevelLayout levelLayout)
        {
            LevelInstance level = _levelGenerator.GenerateLevel(levelLayout);
            StartCoroutine(StartLevelAfterDelay(level));
        }

        private IEnumerator StartLevelAfterDelay(LevelInstance level)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Level started");
            level.StartLevel();
            _users.ApplyFunction(p => p.Player.StartLevel());
        }
    }
}