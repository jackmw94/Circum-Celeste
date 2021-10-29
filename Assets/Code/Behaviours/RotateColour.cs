using UnityEngine;

namespace Code.Behaviours
{
    public class RotateColour : MonoBehaviour
    {
        [SerializeField] private Color _disabledColour;
        [SerializeField] private Colourable _colourable;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _intensity = 1f;

        private void OnDisable()
        {
            _colourable.SetColour(_disabledColour);
        }

        private void Update()
        {
            float red = Mathf.PerlinNoise(Time.time * _speed, 0f);
            float green = Mathf.PerlinNoise(Time.time * _speed, 10000f);
            float blue = Mathf.PerlinNoise(Time.time * _speed, 50000f);
        
            Color colour = new Color(red, green, blue) * _intensity;
            _colourable.SetColour(colour);
        }
    }
}