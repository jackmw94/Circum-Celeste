using System;
using System.Collections;
using UnityEngine;
using UnityExtras.Core;

namespace Code.Level
{
    public abstract class LevelOverlay : MonoBehaviour
    {
        public class OverlayTransitionConfiguration
        {
            public Vector2 Position { get; set; } = Vector2.zero;
            public bool Instant { get; set; } = false;
            public Color TargetColour { get; set; } = Color.white;
            public bool Twirl { get; set; } = false;
        }
        
        [SerializeField] private float _overlayOnExpansionValue = 2.3f;
        [SerializeField] private float _twirlOnValue = 6f;
        [SerializeField] private AnimationCurve _turnOnCurve;
        
        private bool _overlayIsOn = false;
        private Material _material = null;
        private Coroutine _turnOnOffCoroutine;
        private float _currentTransitionValue;
        
        public bool OverlayIsOn => _overlayIsOn;
        
        private static readonly int PointId = Shader.PropertyToID("Point");
        private static readonly int ExpansionId = Shader.PropertyToID("Expansion");
        private static readonly int TwirlId = Shader.PropertyToID("TwirlStrength");
        private static readonly int ColourId = Shader.PropertyToID("Colour");

        public static OverlayTransitionConfiguration DefaultTransitionConfiguration = new OverlayTransitionConfiguration();

        private void Awake()
        {
            _material = GetMaterial();
            HideOverlay(new OverlayTransitionConfiguration
            {
                Instant = true
            });
        }

        protected abstract Material GetMaterial();
        protected abstract void SetColliderOnOff(bool on);

        public void ShowHideOverlay(bool show, OverlayTransitionConfiguration overlayTransitionConfiguration, Action onComplete = null)
        {
            if (show)
            {
                ShowOverlay(overlayTransitionConfiguration, onComplete);
            }
            else
            {
                HideOverlay(overlayTransitionConfiguration, onComplete);
            }
        }

        public void ShowOverlay(OverlayTransitionConfiguration overlayTransitionConfiguration, Action onComplete = null)
        {
            float gridExtent = LevelCellHelper.RealGridDimension * 0.5f;
            Vector2 normalisedPosition = (overlayTransitionConfiguration.Position / gridExtent) / 2f + Vector2.one / 2f;
            Debug.Log($"Showing overlay {overlayTransitionConfiguration.Position} -> {normalisedPosition}");
            _material.SetVector(PointId, normalisedPosition);

            TurnOnOff(true, overlayTransitionConfiguration, onComplete);
        }

        public void HideOverlay(OverlayTransitionConfiguration overlayTransitionConfiguration, Action onComplete = null)
        {
            _material.SetVector(PointId, Vector2.one / 2f);
            TurnOnOff(false, overlayTransitionConfiguration, onComplete);
        }
        
        private void TurnOnOff(bool on, OverlayTransitionConfiguration overlayTransitionConfiguration, Action onComplete)
        {
            SetTwirlOnOff(overlayTransitionConfiguration.Twirl);
            
            if (overlayTransitionConfiguration.Instant)
            {
                _material.SetColor(ColourId, overlayTransitionConfiguration.TargetColour);
                _material.SetFloat(ExpansionId, (on ? 1f : 0f) * _overlayOnExpansionValue);
                _overlayIsOn = on;
                SetColliderOnOff(on);
                _currentTransitionValue = on ? 1f : 0f;
                return;
            }
            
            if (_turnOnOffCoroutine != null)
            {
                StopCoroutine(_turnOnOffCoroutine);
            }
            _turnOnOffCoroutine = StartCoroutine(TurnOnOffCoroutine(on, overlayTransitionConfiguration, onComplete));
        }
        
        private void SetTwirlOnOff(bool twirlOn)
        {
            _material.SetFloat(TwirlId, twirlOn ? _twirlOnValue : 0f);
        }
        
        private IEnumerator TurnOnOffCoroutine(bool on, OverlayTransitionConfiguration overlayTransitionConfiguration, Action onComplete)
        {
            SetColliderOnOff(true);
            Color startColour = _material.GetColor(ColourId);
            Color targetColour = overlayTransitionConfiguration.TargetColour;
            float targetValue = on ? 1f : 0f;

            yield return Utilities.LerpOverTime(_currentTransitionValue, targetValue, _turnOnCurve.GetCurveDuration(), f =>
            {
                _currentTransitionValue = f;
                float directionalF = on ? f : 1f - f;
                float curvedF = _turnOnCurve.Evaluate(f);
                _material.SetFloat(ExpansionId, curvedF * _overlayOnExpansionValue);
                float colourLerpSpeedFactor = 1.5f;
                _material.SetColor(ColourId, Color.Lerp(startColour, targetColour, directionalF * colourLerpSpeedFactor));
            });
            
            _overlayIsOn = on;
            if (!on)
            {
                SetColliderOnOff(false);
            }
            
            onComplete?.Invoke();
        }
    }
}