using System.Collections;
using UnityCommonFeatures;
using UnityEngine;
using UnityExtras.Core;

namespace Code.Level
{
    public class BeamHazardStateMachine : StateMachine
    {
        [SerializeField] private Transform _damagerTransform;
        [SerializeField] private float _damagerResizeOnDuration = 0.15f;
        [SerializeField] private float _damagerResizeOffDuration = 0.05f;
        [Space(15)]
        [SerializeField] private Animator _animator;
        [SerializeField] private string _isWarmingUpAnimatorParameter;
        [SerializeField] private string _isFiringAnimatorParameter;
        [Space(15)]
        [SerializeField] private BeamHazardSettings _beamHazardSettings;

        private int _isWarmingUpAnimatorParameterId;
        private int _isFiringAnimatorParameterId;

        private Coroutine _resizeDamagerCoroutine = null;
        
        public bool LevelStarted { get; set; } = false;
        
        protected override void Awake()
        {
            _isWarmingUpAnimatorParameterId = Animator.StringToHash(_isWarmingUpAnimatorParameter);
            _isFiringAnimatorParameterId = Animator.StringToHash(_isFiringAnimatorParameter);
            
            SetFiringOnOff(false, true);
            
            transform.rotation = Quaternion.Euler(0f, 0f, _beamHazardSettings.StartingAngle);
            
            SetupStateMachine();
        }

        protected override void Update()
        {
            base.Update();

            if (!LevelStarted)
            {
                return;
            }
            
            transform.Rotate(Vector3.forward, _beamHazardSettings.RotationSpeed * Time.deltaTime);
        }

        private void SetupStateMachine()
        {
            State idleState = new State(this);
            State warmUpState = new State(this);
            State firingState = new State(this);
            
            idleState.InitialiseActions(onUpdate: () =>
            {
                bool withinAngle = IsWithinRange(true);
                return LevelStarted && withinAngle ? warmUpState : State.NoTransition;
            });
            
            warmUpState.InitialiseActions(
                onEnter: SetWarmingUp, 
                onUpdate: () => IsWithinRange(false) ? firingState : State.NoTransition);
            
            firingState.InitialiseActions(
                onEnter: () => SetFiringOnOff(true), 
                onUpdate: () => IsWithinRange(false) ? State.NoTransition : idleState, 
                onExit: () => SetFiringOnOff(false));
            
            StartingState = idleState;
        }

        private bool IsWithinRange(bool includeWarmUp)
        {
            bool isMovingClockwise = _beamHazardSettings.RotationSpeed > 0f;
            Vector2 warmupOffset = includeWarmUp ? 
                new Vector2(isMovingClockwise ? -_beamHazardSettings.WarmUpAngle : 0f, isMovingClockwise ? 0f : _beamHazardSettings.WarmUpAngle) : 
                Vector2.zero;
            Vector2 angleRange = _beamHazardSettings.BeamBetweenAngles + warmupOffset;
                
            float currentAngle = transform.rotation.eulerAngles.z;
            bool isWithinAngle = Utilities.IsAngleWithinAngleRange(currentAngle, angleRange);
            if (_beamHazardSettings.TriggerOnBothSides)
            {
                isWithinAngle |= Utilities.IsAngleWithinAngleRange(currentAngle + 180f, angleRange);
            }

            return isWithinAngle;
        }

        private IEnumerator SetDamagerOnOff(bool on, bool instant)
        {
            float startValue = _damagerTransform.localScale.y;
            float targetValue = on ? 1f : 0f;

            float resizeDuration = 0f;
            if (!instant)
            {
                resizeDuration = on ? _damagerResizeOnDuration : _damagerResizeOffDuration;
            }
            
            yield return Utilities.LerpOverTime(startValue, targetValue, resizeDuration, size =>
            {
                _damagerTransform.localScale = new Vector3(1f, size, 1f);
            });
        }

        private void SetWarmingUp()
        {
            _animator.SetTrigger(_isWarmingUpAnimatorParameterId);
        }

        private void SetFiringOnOff(bool isFiring, bool instant = false)
        {
            _animator.SetBool(_isFiringAnimatorParameterId, isFiring);
            this.RestartCoroutine(ref _resizeDamagerCoroutine, SetDamagerOnOff(isFiring, instant));
        }
    }
}