﻿using System;
using System.Collections.Generic;
using Code.Core;
using Code.Debugging;
using Code.Level.Player;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelReplayInstance : LevelInstanceBase
    {
        private Queue<LevelRecordFrameData> _frameReplayData;
        private List<Player.Player> _players;
        private LevelElement[] _levelElements;
        private Action<LevelResult> _levelFinishedCallback;

        private bool _isPlaying = false;

        public void SetupLevel(List<LevelRecordFrameData> levelReplayFrameData, List<Player.Player> players)
        {
            _frameReplayData = new Queue<LevelRecordFrameData>(levelReplayFrameData);
            _players = players;
            _levelElements = GetComponentsInChildren<LevelElement>();

            _players.ApplyFunction(p => p.LevelSetup());
            _levelElements.ApplyFunction(p => p.LevelSetup());
            
            CircumDebug.Log($"Initialised replay with {levelReplayFrameData.Count} values for elements:\n{_levelElements.JoinToString("\n")}");
        }

        public override void LevelReady()
        {
            _players.ApplyFunction(p => p.LevelReady());
            _levelElements.ApplyFunction(p => p.LevelReady());
        }

        public override void StartLevel(Action<LevelResult> levelFinishedCallback)
        {
            _players.ApplyFunction(p => p.LevelStarted());
            _levelElements.ApplyFunction(p => p.LevelStarted());
            
            _isPlaying = true;
            _levelFinishedCallback = levelFinishedCallback;
        }

        private void Update()
        {
            if (!_isPlaying)
            {
                return;
            }
            
            if (_frameReplayData.Count == 0)
            {
                LevelFinished();
            }
        }

        private void FixedUpdate()
        {
            if (!_isPlaying)
            {
                return;
            }
            
            LevelRecordFrameData frameData = _frameReplayData.Dequeue();

            for (int i = 0; i < _levelElements.Length; i++)
            {
                LevelElement levelElement = _levelElements[i];
                levelElement.transform.position = frameData.LevelElementPositions[i];
                levelElement.gameObject.SetActiveSafe(frameData.LevelElementsActive[i]);
            }
        }

        private void LevelFinished()
        {
            _players.ApplyFunction(p => p.LevelFinished());
            _levelElements.ApplyFunction(p => p.LevelFinished());
            
            _isPlaying = false;
            _levelFinishedCallback(null);
        }

        public override Vector3 GetPlayerPosition(int playerIndex)
        {
            return _players[playerIndex].transform.position;
        }
    }
}