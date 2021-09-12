using UnityEngine;
using UnityEngine.UI;

namespace Code.Behaviours
{
    public class AnimateImageShaderProperty : AnimateShaderPropertyBase
    {
        [SerializeField] private Image _image;
        
        protected override Material GetMaterial()
        {
            return _image.material;
        }
    }
}