using System;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class LevelRecordFrameData
    {
        [field: SerializeField] public Vector3 Position { get; set; }
        [field: SerializeField] public float Time { get; set; }
        [field: SerializeField] public bool PowerButtonHeld { get; set; }
    }
}