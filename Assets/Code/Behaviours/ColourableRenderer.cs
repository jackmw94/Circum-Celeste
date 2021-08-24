using System;
using UnityEngine;

namespace Code.Behaviours
{
    [Serializable]
    public class ColourableRenderer : Colourable
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private string _colourPropertyName;

        private int? _colourPropertyId = null;
        private int ColourPropertyId => _colourPropertyId ?? (_colourPropertyId = Shader.PropertyToID(_colourPropertyName)).Value;

        public override Color GetColour()
        {
            return _renderer.material.GetColor(ColourPropertyId);
        }

        public override void SetColour(Color colour)
        {
            _renderer.material.SetColor(ColourPropertyId, colour);
        }
    }
}