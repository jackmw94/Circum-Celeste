using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Level.Player
{
    public class MovementReplayer : InputProvider
    {
        private bool _isReplaying = false;
        private Queue<PositionAtTime> _positions;

        public void Initialise(List<PositionAtTime> positions)
        {
            _positions = new Queue<PositionAtTime>(positions);
        }
        
        public override Vector2 GetMovementInput(Vector3 currentPosition)
        {
            if (_positions == null)
            {
                throw new InvalidOperationException("Cannot get movement input from movement replayer for it's been initialised");
            }
            
            // WARNING: assumes we will call this consistently once per frame
            // if we call this twice then it'll return different values each time and replay at double speed
            // todo: FIX
            
            return _positions.Count > 0 ? (Vector2) (_positions.Dequeue().Position - currentPosition) : Vector2.zero;
        }

        public override bool GetSlingInput()
        {
            return false;
        }
    }
}