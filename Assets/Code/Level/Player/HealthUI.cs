using UnityEngine;

namespace Code.Level.Player
{ 
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [Space(15)]
        [SerializeField] private float _fullColourThreshold = 0.1f;
        [SerializeField] private Color _reduceColour;
        [SerializeField] private Color _increaseColour;

        private Material _material;
        private float _target = 1f;
        private float _current = 1f;
        private static readonly int ColourProperty = Shader.PropertyToID("_BaseColor");

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
            float next = Mathf.Lerp(_current, _target, 0.1f);
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
            _material.SetColor(ColourProperty, colour);
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