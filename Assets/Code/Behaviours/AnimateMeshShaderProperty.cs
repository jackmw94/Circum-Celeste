using UnityEngine;

namespace Code.Behaviours
{
    public class AnimateMeshShaderProperty : AnimateShaderPropertyBase
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        
        protected override Material GetMaterial()
        {
            return _meshRenderer.material;
        }
    }
}