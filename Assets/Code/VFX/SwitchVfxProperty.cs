using UnityEngine;
using UnityEngine.VFX;

namespace Code.VFX
{
    public class SwitchVfxProperty : MonoBehaviour
    {
        [SerializeField] private VisualEffect _visualEffect;
        [SerializeField] private string _propertyName;
        [SerializeField] private float _onValue;
        [SerializeField] private float _offValue;
        [SerializeField] private bool _currentlyOnOff;

        private int _propertyId;

        private void Awake()
        {
            _propertyId = Shader.PropertyToID(_propertyName);
            SetOnOff(_currentlyOnOff);
        }
        
        public void SetOnOff(bool on)
        {
            _currentlyOnOff = on;
            float value = _currentlyOnOff ? _onValue : _offValue;
            _visualEffect.SetFloat(_propertyId, value);
        }
    }
}