using System.Collections.Generic;
using UnityEngine;

namespace Code.Level.Player
{
    public class MovementRecorder : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        
        private bool _isRecording = false;
        private float _duration = 0f;
        
        private readonly List<LevelRecordFrameData> _frameData = new List<LevelRecordFrameData>();
        public List<LevelRecordFrameData> FrameData => _frameData;
        public float Duration => _duration;

        public void StartRecording()
        {
            _isRecording = true;
            _frameData.Clear();
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

            bool powerButtonHeld = _playerInput.InputProvider.GetSlingInput();
            LevelRecordFrameData frameData = new LevelRecordFrameData
            {
                Position = transform.position,
                Time = _duration,
                PowerButtonHeld = powerButtonHeld
            };
            
            _frameData.Add(frameData);
            
            _duration += Time.deltaTime;
        }
    }
}