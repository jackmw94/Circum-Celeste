﻿using UnityEngine;

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