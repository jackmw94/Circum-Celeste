using System;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LevelLayout))]
[CanEditMultipleObjects]
public class LevelLayoutEditor : Editor
{
    private const string LevelCellEmptyTexturePath = "Assets/Editor/Textures/LevelCellEmpty.png";
    private const string LevelCellWallTexturePath = "Assets/Editor/Textures/LevelCellWall.png";
    private const string LevelCellPickupPointTexturePath = "Assets/Editor/Textures/LevelCellPickupPoint.png";
    private const string LevelCellPlayerSpawnTexturePath = "Assets/Editor/Textures/LevelCellPlayerSpawn.png";

    private SerializedProperty _gridSize;
    private SerializedProperty _cells;

    private Texture _levelCellEmptyTexture;
    private Texture _levelCellWallTexture;
    private Texture _levelCellPickupPointTexture;
    private Texture _levelCellPlayerSpawnTexture;

    private void OnEnable()
    {
        _gridSize = serializedObject.FindProperty(nameof(_gridSize));
        _cells = serializedObject.FindProperty(nameof(_cells));

        TryResizeArray();

        _levelCellEmptyTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellEmptyTexturePath);
        _levelCellWallTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellWallTexturePath);
        _levelCellPickupPointTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellPickupPointTexturePath);
        _levelCellPlayerSpawnTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellPlayerSpawnTexturePath);
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        int previousGridSize = _gridSize.intValue;
        EditorGUILayout.PropertyField(_gridSize);
        if (previousGridSize != _gridSize.intValue)
        {
            TryResizeArray();
        }

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

        serializedObject.ApplyModifiedProperties();
    }

    private Texture GetTextureFromCellState(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.None: return _levelCellEmptyTexture;
            case CellType.Wall: return _levelCellWallTexture;
            case CellType.PickupPoint: return _levelCellPickupPointTexture;
            case CellType.PlayerSpawn: return _levelCellPlayerSpawnTexture;
        }

        throw new UnexpectedValuesException($"Could not get texture for cell type {cellType}");
    }

    private CellType GetNextCellType(CellType current)
    {
        int currentInt = (int)current;
        int incrementedInt = currentInt + 1;
        int numCellTypes = Enum.GetNames(typeof(CellType)).Length;
        return incrementedInt >= numCellTypes ? (CellType) 0 : (CellType)incrementedInt;
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
