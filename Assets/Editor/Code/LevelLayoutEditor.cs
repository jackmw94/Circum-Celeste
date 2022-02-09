using System;
using Code.Core;
using Code.Level;
using UnityEditor;
using UnityEngine;
using UnityExtras.Code.Core;

[CustomEditor(typeof(LevelLayout))]
[CanEditMultipleObjects]
public class LevelLayoutEditor : Editor
{
    private const string LevelCellEmptyTexturePath = "Assets/Editor/Textures/LevelCellEmpty.jpg";
    private const string LevelCellWallTexturePath = "Assets/Editor/Textures/LevelCellWall.jpg";
    private const string LevelCellPickupPointTexturePath = "Assets/Editor/Textures/LevelCellPickupPoint.jpg";
    private const string LevelCellEnemyTexturePath = "Assets/Editor/Textures/LevelCellEnemy.jpg";
    private const string LevelCellEscapeTexturePath = "Assets/Editor/Textures/LevelCellEscape.jpg";
    private const string LevelCellPlayerStartTexturePath = "Assets/Editor/Textures/LevelCellPlayerStart.jpg";
    private const string LevelCellHazardTexturePath = "Assets/Editor/Textures/LevelCellHazard.jpg";
    private const string LevelCellBeamHazardTexturePath = "Assets/Editor/Textures/LevelCellBeamHazard.png";

    private SerializedProperty _escapeCriteria;
    private SerializedProperty _playerType;
    private SerializedProperty _escapeTimer;
    private SerializedProperty _tagLineLocalisationTerm;
    private SerializedProperty _tutorialDescription;
    private SerializedProperty _introduceElement;
    private SerializedProperty _exampleRotatingOrbiterEnabled;
    private SerializedProperty _exampleMovingOrbiterEnabled;
    private SerializedProperty _requiredForGameCompletion;
    private SerializedProperty _orbiterEnabled;
    private SerializedProperty _playerInvulnerable;
    private SerializedProperty _gridSize;
    private SerializedProperty _cells;
    private SerializedProperty _goldTime;

    private Texture _levelCellEmptyTexture;
    private Texture _levelCellWallTexture;
    private Texture _levelCellPickupTexture;
    private Texture _levelCellEnemyTexture;
    private Texture _levelCellEscapeTexture;
    private Texture _levelCellPlayerStartTexture;
    private Texture _levelCellHazardTexture;
    private Texture _levelCellBeamHazardTexture;
    
    private void OnEnable()
    {
        _gridSize = serializedObject.FindProperty(nameof(_gridSize));
        _cells = serializedObject.FindProperty(nameof(_cells));
        _goldTime = serializedObject.FindProperty(nameof(_goldTime));

        _playerType = serializedObject.FindProperty(nameof(_playerType));
        _escapeCriteria = serializedObject.FindProperty(nameof(_escapeCriteria));
        _escapeTimer = serializedObject.FindProperty(nameof(_escapeTimer));
        
        _tagLineLocalisationTerm = serializedObject.FindProperty(nameof(_tagLineLocalisationTerm));
        _tutorialDescription = serializedObject.FindProperty(nameof(_tutorialDescription));
        _introduceElement = serializedObject.FindProperty(nameof(_introduceElement));
        _exampleRotatingOrbiterEnabled = serializedObject.FindProperty(nameof(_exampleRotatingOrbiterEnabled));
        _exampleMovingOrbiterEnabled = serializedObject.FindProperty(nameof(_exampleMovingOrbiterEnabled));
        _requiredForGameCompletion = serializedObject.FindProperty(nameof(_requiredForGameCompletion));
        _playerInvulnerable = serializedObject.FindProperty(nameof(_playerInvulnerable));
        _orbiterEnabled = serializedObject.FindProperty(nameof(_orbiterEnabled));

        TryResizeArray();

        _levelCellEmptyTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellEmptyTexturePath);
        _levelCellWallTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellWallTexturePath);
        _levelCellPickupTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellPickupPointTexturePath);
        _levelCellEnemyTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellEnemyTexturePath);
        _levelCellEscapeTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellEscapeTexturePath);
        _levelCellPlayerStartTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellPlayerStartTexturePath);
        _levelCellHazardTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellHazardTexturePath);
        _levelCellBeamHazardTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellBeamHazardTexturePath);
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_tagLineLocalisationTerm);
        GUILayout.Space(15);
        EditorGUILayout.PropertyField(_tutorialDescription);
        GUILayout.Space(15);
        EditorGUILayout.PropertyField(_introduceElement);
        EditorGUILayout.PropertyField(_exampleRotatingOrbiterEnabled);
        EditorGUILayout.PropertyField(_exampleMovingOrbiterEnabled);
        GUILayout.Space(15);
        EditorGUILayout.PropertyField(_playerType);
        EditorGUILayout.PropertyField(_orbiterEnabled);
        EditorGUILayout.PropertyField(_playerInvulnerable);
        EditorGUILayout.PropertyField(_requiredForGameCompletion);
        GUILayout.Space(15);
        EditorGUILayout.PropertyField(_goldTime);
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
        int gridCellDimension = Mathf.FloorToInt(0.9f * EditorGUIUtility.currentViewWidth / _gridSize.intValue);
        
        for (int y = 0; y < _gridSize.intValue; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < _gridSize.intValue; x++)
            {
                SerializedProperty cellProperty = _cells.GetArrayElementAtIndex(cellIndex);
                
                CellType cellState = (CellType)cellProperty.intValue;
                Texture cellTexture = GetTextureFromCellState(cellState);
                bool toggle = GUILayout.Button(new GUIContent(cellTexture), GUILayout.Width(gridCellDimension), GUILayout.Height(gridCellDimension));

                if (toggle)
                {
                    if (Event.current.button == 1)
                    {
                        cellProperty.intValue = (int) GetOffsetCellType(cellState, -1);
                    }
                    else
                    {
                        cellProperty.intValue = (int) GetOffsetCellType(cellState);
                    }
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
            case CellType.Hazard: return _levelCellHazardTexture;
            case CellType.BeamHazard: return _levelCellBeamHazardTexture;
        }

        throw new UnexpectedValuesException($"Could not get texture for cell type {cellType}");
    }

    private CellType GetOffsetCellType(CellType current, int offset = 1)
    {
        int currentInt = (int)current;
        int offsetInt = currentInt + offset;
        int numCellTypes = Enum.GetNames(typeof(CellType)).Length;

        return (CellType)Utilities.Mod(offsetInt, numCellTypes);
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