using System.Collections.Generic;
using System.Linq;
using Code.Level.Player;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelGenerator : MonoBehaviour
    {
        private class LevelObjects
        {
            public List<GameObject> PickupObjects { get; set; }
            public List<GameObject> FollowerEnemyObjects { get; set; }
            public List<GameObject> EscapeObjects { get; set; }
            public List<GameObject> HazardObjects { get; set; }
        }
        
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject _pickupPrefab;
        [SerializeField] private GameObject _followerEnemyPrefab;
        [SerializeField] private GameObject _escapePrefab;
        [SerializeField] private GameObject _hazardPrefab;
        [Space(15)]
        [SerializeField] private Transform _cellsRoot;
        [Space(15)]
        [SerializeField] private GameObject _exampleOrbiter;
        [Space(15)]
        [SerializeField] private float _wallZOffset = 1f;

        public LevelPlayInstance GenerateLevel(InputProvider[] playersInputs, LevelLayout level)
        {
            LevelObjects levelObjects = CreateLevelObjects(level);
            
            // Create and initialise players
            int playerCount = level.GetCellTypeCoordinates(CellType.PlayerStart).Count;
            List<Player.Player> allPlayers = GeneratePlayers(level, playerCount, playersInputs);

            // Get level components from generated objects
            List<Pickup> allPickups = levelObjects.PickupObjects.Select(p => p.GetComponentInChildren<Pickup>()).ToList();
            List<Enemy> allEnemies = levelObjects.FollowerEnemyObjects.Select(p => p.GetComponentInChildren<Enemy>()).ToList();
            List<Escape> allEscapes = levelObjects.EscapeObjects.Select(p => p.GetComponentInChildren<Escape>()).ToList();
            List<Hazard> allHazards = levelObjects.HazardObjects.Select(p => p.GetComponentInChildren<Hazard>()).ToList();

            // Initialise level instance
            LevelPlayInstance levelPlayInstance = _cellsRoot.gameObject.AddComponent<LevelPlayInstance>();
            levelPlayInstance.name = level.name;
            levelPlayInstance.SetupLevel(level, allPlayers, allPickups, allEnemies, allEscapes, allHazards);

            return levelPlayInstance;
        }

        public LevelReplayInstance GenerateReplay(List<LevelRecordFrameData> replay, LevelLayout level)
        {
            CreateLevelObjects(level);
            
            // Create and initialise players
            int playerCount = level.GetCellTypeCoordinates(CellType.PlayerStart).Count;
            List<Player.Player> allPlayers = GeneratePlayers(level, playerCount, null);
            
            // Remove play-only behaviours
            GetComponentsInChildren<LevelPlayBehaviour>().ApplyFunction(Destroy);
            
            // Initialise level replay
            LevelReplayInstance levelReplay = _cellsRoot.gameObject.AddComponent<LevelReplayInstance>();
            levelReplay.name = level.name;
            levelReplay.SetupLevel(replay, allPlayers);

            return levelReplay;
        }

        private LevelObjects CreateLevelObjects(LevelLayout level)
        {
            // Clean up any previous level
            DestroyLevel();
            
            // Create new level objects
            GenerateCells(level, CellType.Wall, _wallPrefab, zOffset: _wallZOffset);
            
            List<GameObject> pickupObjects = GenerateCells(level, CellType.Pickup, _pickupPrefab);
            List<GameObject> followerEnemyObjects = GenerateCells(level, CellType.Enemy, _followerEnemyPrefab);
            List<GameObject> escapeObjects = GenerateCells(level, CellType.Escape, _escapePrefab);
            List<GameObject> hazardObjects = GenerateCells(level, CellType.Hazard, _hazardPrefab);
            
            _exampleOrbiter.SetActive(level.ExampleOrbiterEnabled);

            return new LevelObjects
            {
                EscapeObjects = escapeObjects,
                HazardObjects = hazardObjects,
                PickupObjects = pickupObjects,
                FollowerEnemyObjects = followerEnemyObjects
            };
        }

        private List<Player.Player> GeneratePlayers(LevelLayout level, int numberOfPlayers, InputProvider[] playersInputs)
        {
            List<GameObject> playerObjects = GenerateCells(level, CellType.PlayerStart, _playerPrefab, numberOfPlayers);
            List<Player.Player> allPlayers = playerObjects.Select(p => p.GetComponentInChildren<Player.Player>()).ToList();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                Player.Player player = allPlayers[i];
                InputProvider inputProvider = playersInputs?[i];
                player.Initialise(5, inputProvider, level.OrbiterEnabled, level.PowerEnabled);
            }

            return allPlayers;
        }
    
        private List<GameObject> GenerateCells(LevelLayout level, CellType cellType, GameObject prefab, int limit = -1, float zOffset = 0f)
        {
            List<Vector2Int> cellPositions = level.GetCellTypeCoordinates(cellType);
            
            List<GameObject> cellInstances = new List<GameObject>();
            foreach (Vector2Int cellPosition in cellPositions)
            {
                GameObject cell = Instantiate(prefab, _cellsRoot);
                cellInstances.Add(cell);
                
                LevelCellHelper.Initialise(cell.transform, cellPosition, level.GridSize, zOffset);

                if (limit >= 0 && cellInstances.Count >= limit)
                {
                    return cellInstances;
                }
            }

            return cellInstances;
        }

        public void DestroyLevel()
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