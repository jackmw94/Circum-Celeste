using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Behaviours
{
    [RequireComponent(typeof(Image))]
    public class HDRImage : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField, ColorUsage(true, true)] private Color _colour;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }

            if (_image)
            {
                _image.color = _colour;
            }
        }
    }
}