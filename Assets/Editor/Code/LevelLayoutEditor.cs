using System;
using Code.Core;
using Code.Level;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelLayout))]
[CanEditMultipleObjects]
public class LevelLayoutEditor : Editor
{
    private const string LevelCellEmptyTexturePath = "Assets/Editor/Textures/LevelCellEmpty.png";
    private const string LevelCellWallTexturePath = "Assets/Editor/Textures/LevelCellWall.png";
    private const string LevelCellPickupPointTexturePath = "Assets/Editor/Textures/LevelCellPickupPoint.png";
    private const string LevelCellEnemyTexturePath = "Assets/Editor/Textures/LevelCellEnemy.png";
    private const string LevelCellEscapeTexturePath = "Assets/Editor/Textures/LevelCellEscape.png";
    private const string LevelCellPlayerStartTexturePath = "Assets/Editor/Textures/LevelCellPlayerStart.png";

    private SerializedProperty _escapeCriteria;
    private SerializedProperty _escapeTimer;
    private SerializedProperty _introduceElement;
    private SerializedProperty _gridSize;
    private SerializedProperty _cells;

    private Texture _levelCellEmptyTexture;
    private Texture _levelCellWallTexture;
    private Texture _levelCellPickupTexture;
    private Texture _levelCellEnemyTexture;
    private Texture _levelCellEscapeTexture;
    private Texture _levelCellPlayerStartTexture;

    private void OnEnable()
    {
        _gridSize = serializedObject.FindProperty(nameof(_gridSize));
        _cells = serializedObject.FindProperty(nameof(_cells));
        
        _escapeCriteria = serializedObject.FindProperty(nameof(_escapeCriteria));
        _escapeTimer = serializedObject.FindProperty(nameof(_escapeTimer));
        
        _introduceElement = serializedObject.FindProperty(nameof(_introduceElement));

        TryResizeArray();

        _levelCellEmptyTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellEmptyTexturePath);
        _levelCellWallTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellWallTexturePath);
        _levelCellPickupTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellPickupPointTexturePath);
        _levelCellEnemyTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellEnemyTexturePath);
        _levelCellEscapeTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellEscapeTexturePath);
        _levelCellPlayerStartTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellPlayerStartTexturePath);
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_introduceElement);
        GUILayout.Space(15);
        HandleEscapeCriteriaProperty();
        GUILayout.Space(15);
        HandleGridSizeProperty();
        HandleGrid();

        serializedObject.ApplyModifiedProperties();
    }

    private void HandleEscapeCriteriaProperty()
    {
        EditorGUILayout.PropertyField(_escapeCriteria);
        if (_escapeCriteria.intValue == (int) EscapeCriteria.Timed)
        {
            EditorGUILayout.PropertyField(_escapeTimer);
        }
    }

    private void HandleGridSizeProperty()
    {
        int previousGridSize = _gridSize.intValue;
        EditorGUILayout.PropertyField(_gridSize);
        if (previousGridSize != _gridSize.intValue)
        {
            TryResizeArray();
        }
    }
    
    private void HandleGrid()
    {
        EditorGUILayout.BeginVertical();
        int cellIndex = 0;
        for (int y = 0; y < _gridSize.intValue; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < _gridSize.intValue; x++)
            {
                SerializedProperty cellProperty = _cells.GetArrayElementAtIndex(cellIndex);
                
                CellType cellState = (CellType)cellProperty.intValue;
                Texture cellTexture = GetTextureFromCellState(cellState);
                bool toggle = GUILayout.Button(new GUIContent(cellTexture));

                if (toggle)
                {
                    cellProperty.intValue = (int)GetNextCellType(cellState);
                }

                cellIndex++;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private Texture GetTextureFromCellState(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.None: return _levelCellEmptyTexture;
            case CellType.Wall: return _levelCellWallTexture;
            case CellType.Pickup: return _levelCellPickupTexture;
            case CellType.Enemy: return _levelCellEnemyTexture;
            case CellType.Escape: return _levelCellEscapeTexture;
            case CellType.PlayerStart: return _levelCellPlayerStartTexture;
        }

        throw new UnexpectedValuesException($"Could not get texture for cell type {cellType}");
    }

    private CellType GetNextCellType(CellType current)
    {
        int currentInt = (int)current;
        int incrementedInt = currentInt + 1;
        int numCellTypes = Enum.GetNames(typeof(CellType)).Length;
        return incrementedInt >= numCellTypes ? (CellType)0 : (CellType)incrementedInt;
    }

    private void TryResizeArray()
    {
        int gridCellsCount = _gridSize.intValue * _gridSize.intValue;
        if (_cells.arraySize != gridCellsCount)
        {
            _cells.arraySize = gridCellsCount;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
