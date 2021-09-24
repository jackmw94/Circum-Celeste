using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Debugging;
using Code.Level.Player;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelReplayInstance : LevelInstanceBase
    {
        // frames are recorded until the level is destroyed, we actually finish a few seconds before this
        private const int EndLevelWithFramesRemaining = 0;
        
        private Queue<LevelRecordFrameData> _frameReplayData;
        private List<Player.Player> _players;
        private LevelElement[] _levelElements;
        private Escape[] _escapes;

        private bool _isPlaying = false;
        private bool _hasCalledFinish = false;
        private float _replayTime = 0f;

        public void SetupLevel(LevelRecordingData levelRecordingData, List<Player.Player> players)
        {
            _frameReplayData = new Queue<LevelRecordFrameData>(levelRecordingData.FrameData);
            _players = players;
            _replayTime = levelRecordingData.LevelTime;
            _levelElements = GetComponentsInChildren<LevelElement>();
            _escapes = _levelElements.Where(p => p is Escape).Cast<Escape>().ToArray();

            _players.ApplyFunction(p => p.LevelSetup());
            _levelElements.ApplyFunction(p => p.LevelSetup());
            
            CircumDebug.Log($"Initialised replay with {_frameReplayData.Count} values for elements:\n{_levelElements.JoinToString("\n")}");
        }

        protected override void OnLevelReady()
        {
            _players.ApplyFunction(p => p.LevelReady());
            _levelElements.ApplyFunction(p => p.LevelReady());
        }

        protected override void OnStartLevel()
        {
            _players.ApplyFunction(p => p.LevelStarted());
            _levelElements.ApplyFunction(p => p.LevelStarted());
            
            _isPlaying = true;
        }

        private void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            bool hasHitEscape = _escapes.Any(p => p.IsCollected);

            if (!_hasCalledFinish && (hasHitEscape || _frameReplayData.Count <= EndLevelWithFramesRemaining || _frameReplayData.Count == 0))
            {
                FinishReplay();
            }
        }

        private void FinishReplay()
        {
            ReplayFinished();
            
            LevelRecordingData levelRecordingData = new LevelRecordingData
            {
                FrameData = null,
                LevelTime = _replayTime
            };
            LevelResult result = new LevelResult(false, false, levelRecordingData, true);
            LevelResult levelResult = result;
            
            LevelFinished(levelResult);
            
            _hasCalledFinish = true;
        }

        private void FixedUpdate()
        {
            if (!_isPlaying || _frameReplayData.Count == 0)
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

        private void ReplayFinished()
        {
            _players.ApplyFunction(p => p.LevelFinished());
            _levelElements.ApplyFunction(p => p.LevelFinished());
            
            _isPlaying = false;
        }

        public override Vector3 GetPlayerPosition(int playerIndex)
        {
            return _players[playerIndex].transform.position;
        }
    }
}