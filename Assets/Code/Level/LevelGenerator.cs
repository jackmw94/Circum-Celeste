using System.Collections.Generic;
using System.Linq;
using Code.Level.Player;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject _pickupPrefab;
        [SerializeField] private GameObject _followerEnemyPrefab;
        [SerializeField] private GameObject _escapePrefab;
        [SerializeField] private Transform _cellsRoot;
        
        public LevelInstance GenerateLevel(InputProvider[] playersInputs, LevelLayout level)
        {
            // Clean up any previous level
            DestroyCells();
            
            // Create new level objects
            GenerateCells(level, CellType.Wall, _wallPrefab);
            
            List<GameObject> pickupObjects = GenerateCells(level, CellType.Pickup, _pickupPrefab);
            List<GameObject> followerEnemyObjects = GenerateCells(level, CellType.Enemy, _followerEnemyPrefab);
            List<GameObject> escapeObjects = GenerateCells(level, CellType.Escape, _escapePrefab);
            
            // Create and initialise players
            List<Player.Player> allPlayers = GeneratePlayers(level, playersInputs);

            // Get level components from generated objects
            List<Pickup> allPickups = pickupObjects.Select(p => p.GetComponentInChildren<Pickup>()).ToList();
            List<Enemy> allEnemies = followerEnemyObjects.Select(p => p.GetComponentInChildren<Enemy>()).ToList();
            List<Escape> allEscapes = escapeObjects.Select(p => p.GetComponentInChildren<Escape>()).ToList();

            // Initialise level instance
            LevelInstance levelInstance = _cellsRoot.gameObject.AddComponent<LevelInstance>();
            levelInstance.SetupLevel(level, allPlayers, allPickups, allEnemies, allEscapes);

            return levelInstance;
        }

        private List<Player.Player> GeneratePlayers(LevelLayout level, InputProvider[] playersInputs)
        {
            List<GameObject> playerObjects = GenerateCells(level, CellType.PlayerStart, _playerPrefab, playersInputs.Length);
            List<Player.Player> allPlayers = playerObjects.Select(p => p.GetComponentInChildren<Player.Player>()).ToList();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                Player.Player player = allPlayers[i];
                InputProvider inputProvider = playersInputs[i];
                player.Initialise(5, inputProvider);
            }

            return allPlayers;
        }
    
        private List<GameObject> GenerateCells(LevelLayout level, CellType cellType, GameObject prefab, int limit = -1)
        {
            List<Vector2Int> cellPositions = level.GetCellTypeCoordinates(cellType);
            
            List<GameObject> cellInstances = new List<GameObject>();
            foreach (Vector2Int cellPosition in cellPositions)
            {
                GameObject cell = Instantiate(prefab, _cellsRoot);
                cellInstances.Add(cell);
                
                LevelCellHelper.Initialise(cell.transform, cellPosition, level.GridSize);

                if (limit >= 0 && cellInstances.Count >= limit)
                {
                    return cellInstances;
                }
            }

            return cellInstances;
        }

        private void DestroyCells()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _cellsRoot.DestroyAllChildrenInEditor();
                _cellsRoot.GetComponents<Behaviour>().ApplyFunction(DestroyImmediate);
            }
#endif

            _cellsRoot.DestroyAllChildren();
            _cellsRoot.GetComponents<Behaviour>().ApplyFunction(Destroy);
        }
    }
}