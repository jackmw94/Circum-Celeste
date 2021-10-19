﻿using System;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class LevelMetaData
    {
        [SerializeField] private float _levelTime;
        [SerializeField] private bool _isPerfect;

        public LevelMetaData(LevelStats levelStats)
        {
            _levelTime = !levelStats.HasRecording ? -1f : levelStats.LevelRecording.LevelTime;
            _isPerfect = levelStats.HasRecording && levelStats.LevelRecording.IsPerfect;
        }
            
        public bool HasCompletedLevel => _levelTime > 0f;
        public bool HasPerfectTime => _isPerfect;
    }
}