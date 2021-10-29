using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace Code.Behaviours
{
    public class ColourableTextMeshProUGUI : Colourable
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_textMeshProUGUI)
            {
                _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            }
        }


        public override Color GetColour()
        {
            return _textMeshProUGUI.color;
        }

        public override void SetColour(Color colour)
        {
            _textMeshProUGUI.color = colour;
        }
    }
}