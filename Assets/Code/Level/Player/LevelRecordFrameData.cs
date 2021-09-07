using System;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public struct LevelRecordFrameData
    {
        [field: SerializeField] public Vector3[] LevelElementPositions { get; set; }
        [field: SerializeField] public bool[] LevelElementsActive { get; set; }
    }
}