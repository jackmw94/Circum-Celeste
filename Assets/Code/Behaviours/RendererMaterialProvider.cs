using System.Diagnostics;
using UnityEngine;

namespace Code.Behaviours
{
    public class RendererMaterialProvider : MaterialProvider
    {
        [SerializeField] private Renderer _renderer;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_renderer)
            {
                _renderer = GetComponent<Renderer>();
            }
        }

        public override Material GetMaterial()
        {
            return _renderer.material;
        }
    }
}