using UnityEngine;
using UnityEngine.UI;

namespace Code.Behaviours
{
    public class AnimateGraphicShaderProperty : AnimateShaderPropertyBase
    {
        [SerializeField] private Graphic _image;
        
        protected override Material GetMaterial()
        {
            return _image.material;
        }
    }
}