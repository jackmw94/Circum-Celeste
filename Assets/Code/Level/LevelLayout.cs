using System.Collections.Generic;
using Code.Core;
using UnityEngine;

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create LevelLayout", fileName = "LevelLayout", order = 0)]
    public class LevelLayout : ScriptableObject
    {
        // arbitrary, high enough for variation
        private const int SeedMaxValue = 100;

        [SerializeField] private int _maxPickupsToSpawnAtStart = 3;
        [SerializeField] private int _gridSize = 10;
        [SerializeField] private CellType[] _cells = new CellType[0];

        public int MaxPickupsToSpawnAtStart => _maxPickupsToSpawnAtStart;
        public int GridSize => _gridSize;
        public CellType[] Cells => _cells;
        
        public List<Vector2Int> GetCellTypeCoordinates(CellType cellType)
        {
            if (_cells.Length != _gridSize * _gridSize)
            {
                throw new UnexpectedValuesException($"Could not get level data as 2d array since cell values (count={_cells.Length}) was not the square of grid size {_gridSize}");
            }

            List<Vector2Int> cellCoordinates = new List<Vector2Int>();
            for (int x = 0; x < _gridSize; x++)
            {
                for (int y = 0; y < _gridSize; y++)
                {
                    int cellIndex = CellCoordinateToCellIndex(x, y);
                    if (_cells[cellIndex] == cellType)
                    {
                        cellCoordinates.Add(new Vector2Int(x, y));
                    }
                }
            }

            return cellCoordinates;
        }

        private int CellCoordinateToCellIndex(int x, int y)
        {
            return x + y * _gridSize;
        }

        private Vector2Int CellIndexToCellCoordinate(int index)
        {
            // loss of fraction intended
            int y = index / _gridSize;
        
            int x = index - y * _gridSize;

            return new Vector2Int(x, y);
        }

        [ContextMenu(nameof(ResetLevelData))]
        private void ResetLevelData()
        {
            _cells = new CellType[0];
        }
    }
}