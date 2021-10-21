using System.Collections.Generic;
using System.Diagnostics;
using Code.Core;
using Code.Debugging;
using UnityEngine;
using UnityEngine.Serialization;
using UnityExtras.Code.Core;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create LevelLayout", fileName = "LevelLayout", order = 0)]
    public class LevelLayout : ScriptableObject
    {
        [SerializeField] private long _levelId = 0;
        [SerializeField] private EscapeCriteria _escapeCriteria;
        [SerializeField] private float _escapeTimer = 25f;
        [FormerlySerializedAs("_tagLine")] [SerializeField] private string _tagLineLocalisationTerm = "";
        [SerializeField] private bool _orbiterEnabled = true;
        [SerializeField] private bool _playerInvulnerable = false;
        [SerializeField] private bool _requiredForGameCompletion = true;
        [SerializeField] private IntroduceElement _introduceElement;
        [SerializeField] private bool _exampleRotatingOrbiterEnabled = false;
        [SerializeField] private bool _exampleMovingOrbiterEnabled = false;
        [SerializeField] private int _gridSize = 10;
        [SerializeField] private float _goldTime = 2f;
        [SerializeField] private CellType[] _cells = new CellType[0];

        public long LevelId => _levelId;
        public int GridSize => _gridSize;
        public EscapeCriteria EscapeCriteria => _escapeCriteria;
        public float EscapeTimer => _escapeTimer;
        public string TagLineLocalisationTerm => _tagLineLocalisationTerm;
        public bool OrbiterEnabled => _orbiterEnabled;
        public bool PlayerInvulnerable => _playerInvulnerable;
        public bool RequiredForGameCompletion => _requiredForGameCompletion;
        public IntroduceElement IntroduceElement => _introduceElement;
        public bool ExampleRotatingOrbiterEnabled => _exampleRotatingOrbiterEnabled;
        public bool ExampleMovingOrbiterEnabled => _exampleMovingOrbiterEnabled;
        public float GoldTime => _goldTime;

        // only runtime data, keeps level indices hidden away in level provider
        // todo: move this out of here, return an object containing LevelLayout and LevelContext instead of where we return LevelContext
        public LevelLayoutContext LevelContext { get; } = new LevelLayoutContext();

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (_levelId == 0)
            {
                RegenerateLevelId();
            }
        }

        [ContextMenu(nameof(RegenerateLevelId))]
        private void RegenerateLevelId()
        {
            _levelId = Utilities.RandomLong(Random.Range(int.MinValue, int.MaxValue));
        }

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
            Debug.Assert(GetCellTypeCoordinates(CellType.Escape).Count > 0, $"There are no escapes in level {name}");
            Debug.Assert(GetCellTypeCoordinates(CellType.PlayerStart).Count > 0, $"There is no player start point in level {name}");
            Debug.Assert(GetCellTypeCoordinates(CellType.PlayerStart).Count < 2, $"There are multiple player start points in level {name}");
        }
    }
}