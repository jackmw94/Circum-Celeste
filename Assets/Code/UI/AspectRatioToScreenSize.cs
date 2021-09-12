﻿using System.Diagnostics;
using UnityEngine;

namespace Code.UI
{
    [ExecuteAlways]
    public class AspectRatioToScreenSize : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [Space(15)]
        [SerializeField] private float _minAspectRatio;
        [SerializeField] private float _minScreenSize;
        [SerializeField] private float _maxAspectRatio;
        [SerializeField] private float _maxScreenSize;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_camera)
            {
                _camera = GetComponent<Camera>();
            }
        }

        private void Awake()
        {
            UpdateScreenSize();
        }

        private void UpdateScreenSize()
        {
            float aspectRatio = Screen.height / (float)Screen.width;
            float lerpVal = Mathf.InverseLerp(_minAspectRatio, _maxAspectRatio, aspectRatio);
            float screenSize = Mathf.Lerp(_minScreenSize, _maxScreenSize, lerpVal);
            _camera.orthographicSize = screenSize;
        }
    
#if UNITY_EDITOR
        private void Update()
        {
            UpdateScreenSize();
        }
#endif
    }
}