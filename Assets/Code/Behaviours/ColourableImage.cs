using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Behaviours
{
    [Serializable]
    public class ColourableImage : Colourable
    {
        [SerializeField] private Image _image;
        
        public override Color GetColour()
        {
            return _image.color;
        }

        public override void SetColour(Color colour)
        {
            _image.color = colour;
        }
    }
}