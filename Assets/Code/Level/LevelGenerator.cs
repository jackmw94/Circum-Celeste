using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject _pickupSpawnerPrefab;
        [SerializeField] private GameObject _followerEnemyPrefab;
        [SerializeField] private GameObject _escapePrefab;
        [SerializeField] private Transform _cellsRoot;
        
        public LevelInstance GenerateLevel(LevelLayout level)
        {
            // Clean up any previous level
            DestroyCells();
            _cellsRoot.GetComponents<Behaviour>().ApplyFunction(Destroy);
            
            // Create new level objects
            GenerateCells(level, CellType.Wall, _wallPrefab);
            List<GameObject> pickupObjects = GenerateCells(level, CellType.Pickup, _pickupSpawnerPrefab);
            List<GameObject> followerEnemyObjects = GenerateCells(level, CellType.Enemy, _followerEnemyPrefab);
            List<GameObject> escapeObjects = GenerateCells(level, CellType.Escape, _escapePrefab);

            // Initialise new level instance
            LevelInstance levelInstance = _cellsRoot.gameObject.AddComponent<LevelInstance>();
            List<Pickup> allPickups = pickupObjects.Select(p => p.GetComponentInChildren<Pickup>()).ToList();
            List<Enemy> allEnemies = followerEnemyObjects.Select(p => p.GetComponent<Enemy>()).ToList();
            List<Escape> allEscapes = escapeObjects.Select(p => p.GetComponent<Escape>()).ToList();
            levelInstance.SetupLevel(level, allPickups, allEnemies, allEscapes);

            return levelInstance;
        }
    
        private List<GameObject> GenerateCells(LevelLayout level, CellType cellType, GameObject prefab)
        {
            List<Vector2Int> cellPositions = level.GetCellTypeCoordinates(cellType);
            
            List<GameObject> cellInstances = new List<GameObject>();
            foreach (Vector2Int cellPosition in cellPositions)
            {
                GameObject cell = Instantiate(prefab, _cellsRoot);
                cellInstances.Add(cell);

                LevelCell levelCell = cell.GetComponent<LevelCell>();
                levelCell.Initialise(cellPosition, level.GridSize);
            }

            return cellInstances;
        }

        private void DestroyCells()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _cellsRoot.DestroyAllChildrenInEditor();
                return;
            }
#endif
            _cellsRoot.DestroyAllChildren();
        }
    }
}