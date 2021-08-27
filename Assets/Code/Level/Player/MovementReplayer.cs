using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Level.Player
{
    public class MovementReplayer : InputProvider
    {
        private int _lastRequestedFrame = 0;
        private LevelRecordFrameData _currentFrameData = null;
        
        private Queue<LevelRecordFrameData> _positions;

        public void Initialise(List<LevelRecordFrameData> positions)
        {
            _positions = new Queue<LevelRecordFrameData>(positions);
        }
        
        public override Vector2 GetMovementInput(Vector3 currentPosition)
        {
            if (_positions == null)
            {
                throw new InvalidOperationException("Cannot get movement input from movement replayer for it's been initialised");
            }
            
            // WARNING: assumes we will call this consistently once per frame.
            // The cached _movement allows us to call multiple times per frame but if we call fewer than once per frame we will alter the framerate of the replay

            UpdateFrameData();
            
            Vector2 movement = _currentFrameData == null ? Vector2.zero : (Vector2)(_currentFrameData.Position - currentPosition);
            return movement;
        }

        public override bool GetSlingInput()
        {
            UpdateFrameData();
            return _currentFrameData?.PowerButtonHeld ?? false;
        }

        private void UpdateFrameData()
        {
            int currentFrame = Time.frameCount;
            if (currentFrame == _lastRequestedFrame)
            {
                return;
            }
            
            _lastRequestedFrame = currentFrame;
            _currentFrameData = _positions.Count > 0 ? _positions.Dequeue() : null;
        }
    }
}