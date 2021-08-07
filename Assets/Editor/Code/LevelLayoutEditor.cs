using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LevelLayout))]
[CanEditMultipleObjects]
public class LevelLayoutEditor : Editor
{
    private const string LevelCellOnTexturePath = "Assets/Editor/Textures/LevelCellOn.png";
    private const string LevelCellOffTexturePath = "Assets/Editor/Textures/LevelCellOff.png";
    
    private SerializedProperty _gridSize;
    private SerializedProperty _cellIndices;

    private Texture _levelCellOnTexture;
    private Texture _levelCellOffTexture;
    
    private void OnEnable()
    {
        _gridSize = serializedObject.FindProperty(nameof(_gridSize));
        _cellIndices = serializedObject.FindProperty(nameof(_cellIndices));

        TryResizeArray();

        _levelCellOnTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellOnTexturePath);
        _levelCellOffTexture = AssetDatabase.LoadAssetAtPath<Texture>(LevelCellOffTexturePath);
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
                SerializedProperty cellProperty = _cellIndices.GetArrayElementAtIndex(cellIndex);
                
                bool cellState = cellProperty.boolValue;
                Texture cellTexture = cellState ? _levelCellOnTexture : _levelCellOffTexture;
                bool toggle = GUILayout.Button(new GUIContent(cellTexture));

                if (toggle)
                {
                    cellProperty.boolValue = !cellState;
                }

                cellIndex++;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void TryResizeArray()
    {
        int gridCellsCount = _gridSize.intValue * _gridSize.intValue;
        if (_cellIndices.arraySize != gridCellsCount)
        {
            _cellIndices.arraySize = gridCellsCount;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
