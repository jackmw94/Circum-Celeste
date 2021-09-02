using System.Collections.Generic;
using Code.Core;
using Code.Debugging;
using Code.Level.Player;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    // arbitrary high number, should execute last to ensure all element transforms are updates
    [DefaultExecutionOrder(100)]
    public class LevelRecorder : MonoBehaviour
    {
        private float _levelTime = 0f;
        private readonly List<LevelRecordFrameData> _frameData = new List<LevelRecordFrameData>();

        private LevelElement[] _levelElements;
        
        // pauses recording
        public bool RecordingActive { private get; set; } = true;
        
        public List<LevelRecordFrameData> FrameData => _frameData;
        public float LevelTime => _levelTime;

        private void Awake()
        {
            _levelElements = GetComponentsInChildren<LevelElement>(true);
            CircumDebug.Log($"Started recording with following level elements: \n{_levelElements.JoinToString("\n")}");
        }

        private void FixedUpdate()
        {
            if (!RecordingActive)
            {
                return;
            }

            Vector3[] elementPositions = new Vector3[_levelElements.Length];
            bool[] elementsActive = new bool[_levelElements.Length];
            
            for (int i = 0; i < _levelElements.Length; i++)
            {
                LevelElement element = _levelElements[i];
                elementPositions[i] = element.transform.position;
                elementsActive[i] = element.gameObject.activeSelf;
            }
            
            LevelRecordFrameData levelRecordFrameData = new LevelRecordFrameData()
            {
                LevelElementPositions = elementPositions,
                LevelElementsActive = elementsActive
            };
            
            _frameData.Add(levelRecordFrameData);
            _levelTime += Time.fixedDeltaTime;
        }
    }
}