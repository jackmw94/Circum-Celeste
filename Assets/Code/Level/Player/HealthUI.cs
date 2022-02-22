using UnityEngine;
using UnityExtras.Core;

namespace Code.Level.Player
{ 
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private string _colourPropertyName = "Colour";
        [Space(15)] 
        [SerializeField] private float _lerpAmount = 0.1f;
        [SerializeField] private float _fullColourThreshold = 0.1f;
        [SerializeField, ColorUsage(true, true)] private Color _reduceColour;
        [SerializeField, ColorUsage(true, true)] private Color _increaseColour;
        [Space(15)]
        [SerializeField] private float _defaultSize = 0.15f;
        [SerializeField] private float _expandedSize = 0.3f;
        
        private Material _material;
        private float _target = 1f;
        private float _current = 1f;
        private int? _colourPropertyId = null;
        private int ColourPropertyId => _colourPropertyId ?? (_colourPropertyId = Shader.PropertyToID(_colourPropertyName)).Value;

        private void Awake()
        {
            _material = _meshRenderer.material;
        }
        
        public void UpdateHealthBar(int playerIndex, float fraction)
        {
            _target = fraction;
        }

        private void Update()
        {
            float next = Mathf.Lerp(_current, _target, _lerpAmount);
            float difference = next - _current;
            _current = next;

            UpdateColour(difference);
            UpdateTransform();
        }

        public void SetExpandedSize(bool isExpanded)
        {
            float verticalSize = isExpanded ? _expandedSize : _defaultSize;
            Vector3 localScale = transform.localScale;
            transform.localScale = localScale.ModifyVectorElement(1, verticalSize);
        }

        private void UpdateColour(float frameValueDifference)
        {
            float colourFraction = Mathf.InverseLerp(0f, _fullColourThreshold, Mathf.Abs(frameValueDifference));
            Color fullColour = frameValueDifference < 0 ? _reduceColour : _increaseColour;
            Color colour = Color.Lerp(Color.white, fullColour, colourFraction);
            _material.SetColor(ColourPropertyId, colour);
        }

        private void UpdateTransform()
        {
            Transform t = transform;
            Vector3 localScale = t.localScale;
            localScale.x = _current;
            t.localScale = localScale;
        }
    }
}