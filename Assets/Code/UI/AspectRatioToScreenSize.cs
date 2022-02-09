using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
            if (Screen.width == 0)
            {
                Debug.LogError("Not updating screen size while screen width is zero");
                return;
            }
            
            float aspectRatio = Screen.height / (float)Screen.width;
            float lerpVal = Mathf.InverseLerp(_minAspectRatio, _maxAspectRatio, aspectRatio);
            float screenSize = Mathf.Lerp(_minScreenSize, _maxScreenSize, lerpVal);

            if (screenSize.Equals(0f))
            {
                Debug.LogError("Calculating orthographic size resulted in a value of 0! Returning early");
                return;
            }
            
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