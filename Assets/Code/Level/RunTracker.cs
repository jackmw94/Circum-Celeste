using System;
using Code.Debugging;
using UnityEditor;
using UnityEngine;

namespace Code.Level
{
    [Serializable]
    public class RunTracker
    {
        [field: SerializeField] public int LevelIndex { get; set; } = 0;
        [field: SerializeField] public bool HasSkipped { get; set; } = false;
        [field: SerializeField] public bool IsPerfect { get; set; } = true;
        [field: SerializeField] public int Deaths { get; set; } = 0;

        public void ResetRun()
        {
            HasSkipped = false;
            IsPerfect = true;
            Deaths = 0;
            LevelIndex = 0;
        }

        public override string ToString()
        {
            return $"Level={LevelIndex}, HasSkipped={HasSkipped}, IsPerfect={IsPerfect}, Deaths={Deaths}";
        }

        [MenuItem("Circum/Serialize test")]
        public static void SerializeTest()
        {
            RunTracker runTracker = new RunTracker()
            {
                Deaths = 3,
                HasSkipped = false,
                IsPerfect = false
            };
            
            CircumDebug.Log(JsonUtility.ToJson(runTracker));
        }
    }
}