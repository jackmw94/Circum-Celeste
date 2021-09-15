using System.Diagnostics;
using Code.Core;
using UnityEngine;
using UnityEngine.VFX;

namespace Code.VFX
{
    public abstract class SwitchVfxPropertyBase<T> : MonoBehaviour, ISwitchable
    {
        [SerializeField] private bool _switchActive = true;
        [SerializeField] private VisualEffect _visualEffect;
        [SerializeField] private string _propertyName;
        [SerializeField] private T _onValue;
        [SerializeField] private T _offValue;
        [SerializeField] private bool _currentlyOnOff;

        private int _propertyId;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_visualEffect)
            {
                _visualEffect = GetComponent<VisualEffect>();
            }
        }

        private void Awake()
        {
            _propertyId = Shader.PropertyToID(_propertyName);
            SetOnOff(_currentlyOnOff);
        }

        public void SetOnOff(bool on)
        {
            if (!_switchActive)
            {
                return;
            }
            
            _currentlyOnOff = on;
            T value = _currentlyOnOff ? _onValue : _offValue;
            ApplyValue(_visualEffect, _propertyId, value);
        }

        protected abstract void ApplyValue(VisualEffect vfx, int propertyId, T value);
    }
}