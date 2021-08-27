using System;
using System.Collections.Generic;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create LevelLayout", fileName = "LevelLayout", order = 0)]
    public class LevelLayout : ScriptableObject
    {
        // arbitrary, high enough for variation
        private const int SeedMaxValue = 100;

        [SerializeField] private EscapeCriteria _escapeCriteria;
        [SerializeField] private float _escapeTimer = 25f;
        [SerializeField] private string _tagLine = "";
        [SerializeField] private bool _orbiterEnabled = true;
        [SerializeField] private bool _powerEnabled = false;
        [SerializeField] private IntroduceElement _introduceElement;
        [SerializeField] private bool _exampleOrbiterEnabled = false;
        [SerializeField] private int _gridSize = 10;
        [SerializeField] private CellType[] _cells = new CellType[0];
        
        public int GridSize => _gridSize;
        public EscapeCriteria EscapeCriteria => _escapeCriteria;
        public float EscapeTimer => _escapeTimer;
        public string TagLine => _tagLine;
        public bool OrbiterEnabled => _orbiterEnabled;
        public bool PowerEnabled => _powerEnabled;
        public IntroduceElement IntroduceElement => _introduceElement;
        public bool ExampleOrbiterEnabled => _exampleOrbiterEnabled;
        
        // only runtime data, keeps level indices hidden away in level provider
        // todo: move this out of here, return an object containing LevelLayout and LevelContext instead of where we return LevelContext
        public LevelLayoutContext LevelContext { get; } = new LevelLayoutContext();

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

        public void Validate()
        {
            CircumDebug.Assert(GetCellTypeCoordinates(CellType.Escape).Count > 0, $"There are no escapes in level {name}");
            CircumDebug.Assert(GetCellTypeCoordinates(CellType.PlayerStart).Count > 0, $"There is no player start point in level {name}");
            CircumDebug.Assert(GetCellTypeCoordinates(CellType.PlayerStart).Count < 2, $"There are multiple player start points in level {name}");
        }
    }
}