using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Behaviours
{
    public class ImageMaterialProvider : MaterialProvider
    {
        [SerializeField] private Image _image;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }
        }
        
        public override Material GetMaterial()
        {
            return _image.material;
        }
    }
}