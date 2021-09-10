using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    public class SlingController : LevelPlayBehaviour
    {
        [SerializeField] private float _integralOffset = 0.7f;
        [SerializeField] private float _proportionalOffset = -0.2f;
        [SerializeField] private float _warmUpTime = 0.1f;
        [SerializeField] private float _warmDownTime = 0.5f;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private OrbiterMover _orbitMover;

        private float _currentLerpValue = 0f;
        
        public bool SlingEnabled { get; set; }

        private void Awake()
        {
            _integralOffset = RemoteConfigHelper.SlingIntegralOffset;
            _proportionalOffset = RemoteConfigHelper.SlingProportionalOffset;
        }
    
        private void Update()
        {
            bool slingInput = false;
            if (SlingEnabled)
            {
                slingInput = _playerInput.InputProvider.GetSlingInput();
            }
            
            if (slingInput)
            {
                _currentLerpValue += Time.deltaTime / _warmUpTime;
            }
            else
            {
                _currentLerpValue -= Time.deltaTime / _warmDownTime;
            }

            _currentLerpValue = Mathf.Clamp01(_currentLerpValue);

            float integralOffset = Mathf.Lerp(0f, _integralOffset, _currentLerpValue);
            float proportionalOffset = Mathf.Lerp(0f, _proportionalOffset, _currentLerpValue);
        
            _orbitMover.SetSlingOffsets(integralOffset, proportionalOffset);
        }
    }
}