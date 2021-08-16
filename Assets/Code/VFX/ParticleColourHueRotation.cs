using UnityEngine;
using UnityEngine.VFX;

namespace Code.VFX
{
    public class ParticleColourHueRotation : MonoBehaviour
    {
        [SerializeField] private VisualEffect _visualEffect;
        [SerializeField] private string _propertyName;
        [Space(15)]
        [SerializeField] private float _speed;

        private int _propertyHash;

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            _propertyHash = Shader.PropertyToID(_propertyName);
        }

        private void Awake()
        {
            _propertyHash = Shader.PropertyToID(_propertyName);
        }

        private void Update()
        {
            Vector4 currColourVector = _visualEffect.GetVector4(_propertyHash);
            Color currColour = new Color(currColourVector.x, currColourVector.y, currColourVector.z, currColourVector.w);
            Color.RGBToHSV(currColour, out float hue, out float sat, out float val);
            hue += Time.deltaTime * _speed;
            Color nextColour = Color.HSVToRGB(hue, sat, val);
            Vector4 nextColourVector = new Vector4(nextColour.r, nextColour.g, nextColour.b);
            _visualEffect.SetVector4(_propertyHash, nextColourVector);
        }
    }
}