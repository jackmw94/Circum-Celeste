using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelLayout", fileName = "LevelLayout", order = 0)]
public class LevelLayout : ScriptableObject
{
    [SerializeField] private int _gridSize = 10;
    [SerializeField] private bool[] _cellIndices = new bool[0];
}