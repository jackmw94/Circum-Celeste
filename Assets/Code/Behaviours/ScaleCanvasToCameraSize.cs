using System.Diagnostics;
using UnityEngine;

namespace Code.Behaviours
{
    public class ScaleCanvasToCameraSize : MonoBehaviour
    {
        [SerializeField] private float _unitScaleSize = 1000;
        [SerializeField] private Camera _camera;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            Resize();
        }

        private void Start()
        {
            Resize();
        }

        [ContextMenu(nameof(Resize))]
        private void Resize()
        {
            float currentCameraSize = _camera.orthographicSize;
            float factor = currentCameraSize / _unitScaleSize;
            transform.localScale = Vector3.one * factor;
        }
    }
}