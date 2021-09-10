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
        private bool _propertyIsInt;

        private void Awake()
        {
            _propertyId = Shader.PropertyToID(_propertyName);
            _propertyIsInt = _visualEffect.HasInt(_propertyId);
            SetOnOff(_currentlyOnOff);
        }

        public void SetOnOff(bool on)
        {
            _currentlyOnOff = on;
            float value = _currentlyOnOff ? _onValue : _offValue;

            if (!_propertyIsInt)
            {
                _visualEffect.SetFloat(_propertyId, value);
                return;
            }
            
            _visualEffect.SetInt(_propertyId, Mathf.RoundToInt(value));
        }
    }
}