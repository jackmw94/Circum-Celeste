using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelContainer _levelContainer;
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _pickupSpawnerPrefab;
    [SerializeField] private Transform _cellsRoot;

    public LevelLayout LevelLayout => _levelContainer.LevelLayout;

    private void Awake()
    {
        GenerateLevel();
    }

    [ContextMenu(nameof(GenerateLevel))]
    private void GenerateLevel()
    {
        DestroyCells();

        GenerateCells(CellType.Wall, _wallPrefab);
        GenerateCells(CellType.PickupPoint, _pickupSpawnerPrefab);
    }
    
    private void GenerateCells(CellType cellType, GameObject prefab)
    {
        List<Vector2Int> cellPositions = LevelLayout.GetCellTypeCoordinates(cellType);
        foreach (Vector2Int cellPosition in cellPositions)
        {
            GameObject cell = Instantiate(prefab, _cellsRoot);

            LevelCell levelCell = cell.GetComponent<LevelCell>();
            levelCell.Initialise(cellPosition, LevelLayout.GridSize);
        }
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