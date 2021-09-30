using System;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class LevelMetaData
    {
        [SerializeField] private float _perfectTime;
        [SerializeField] private float _imperfectTime;

        public LevelMetaData(LevelStats levelStats)
        {
            _perfectTime = levelStats.FastestPerfectLevelRecording?.RecordingData?.LevelTime ?? -1f;
            _imperfectTime = levelStats.FastestLevelRecording?.RecordingData?.LevelTime ?? -1f;
        }
            
        public bool HasCompletedLevel => _imperfectTime > 0f;
        public bool HasPerfectTime => _perfectTime > 0f;
    }
}