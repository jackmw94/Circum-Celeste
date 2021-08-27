using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Level.Player
{
    public class MovementRecorder : MonoBehaviour
    {
        private bool _isRecording = false;
        private float _duration = 0f;
        
        private readonly List<PositionAtTime> _positions = new List<PositionAtTime>();
        public List<PositionAtTime> Positions => _positions;
        public float Duration => _duration;

        public void StartRecording()
        {
            _isRecording = true;
            _positions.Clear();
            _duration = 0f;

        }

        public void StopRecording()
        {
            _isRecording = false;
        }

        private void FixedUpdate()
        {
            if (!_isRecording)
            {
                return;
            }
            _positions.Add(new PositionAtTime
            {
                Position = transform.position,
                Time = _duration
            });
            _duration += Time.deltaTime;
        }
    }

    [Serializable]
    public class PositionAtTime
    {
        [field: SerializeField] public Vector3 Position { get; set; }
        [field: SerializeField] public float Time { get; set; }
    }
}