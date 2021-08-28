using System;
using System.Collections.Generic;
using Code.Level.Player;
using UnityEngine;

namespace Code.Level
{
    [Serializable]
    public class LevelRecording
    {
        [field: SerializeField] public int LevelIndex;
        [field: SerializeField] public LevelRecordingData RecordingData;

        public override string ToString()
        {
            return $"---\nLevel index {LevelIndex}\n{(RecordingData == null ? "NULL" : RecordingData.ToString())}\n---";
        }
    }

    [Serializable]
    public class LevelRecordingData
    {
        [field: SerializeField] public List<LevelRecordFrameData> FrameData;
        [field: SerializeField] public float LevelTime = float.MaxValue;

        public override string ToString()
        {
            return $"Level time: {LevelTime}. Frame count = {(FrameData == null ? "NULL" : FrameData.Count.ToString())}";
        }
    }
}