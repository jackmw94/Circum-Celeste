using System.Collections;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelOverlay : MonoBehaviour
    {
        private const float OverlayOnExpansionValue = 1.5f;
        
        [SerializeField] private Renderer _renderer;
        [SerializeField] private AnimationCurve _turnOnCurve;
        [SerializeField] private AnimationCurve _turnOffCurve;

        [SerializeField] private bool _overlayIsOn = false;
        private Material _material = null;

        private Coroutine _turnOnOffCoroutine;
        
        public bool OverlayIsOn => _overlayIsOn;
        
        private static readonly int Point = Shader.PropertyToID("Point");
        private static readonly int Expansion = Shader.PropertyToID("Expansion");

        private void Awake()
        {
            _material = _renderer.material;
            HideOverlay(true);
        }
        
        public void ShowOverlay(Vector2 position, bool instant = false)
        {
            if (_overlayIsOn)
            {
                return;
            }
            
            float gridExtent = LevelCellHelper.RealGridDimension * 0.5f;
            Vector2 normalisedPosition = (position / gridExtent) / 2f + Vector2.one / 2f;
            _material.SetVector(Point, normalisedPosition);

            TurnOnOff(true, instant);
        }

        public void HideOverlay(bool instant = false)
        {
            if (!_overlayIsOn)
            {
                return;
            }

            _material.SetVector(Point, Vector2.one / 2f);
            TurnOnOff(false, instant);
        }
        
        private void TurnOnOff(bool on, bool instant)
        {
            if (instant)
            {
                SetExpansionAmount(on ? 1f : 0f);
                _overlayIsOn = on;
                return;
            }
            
            if (_turnOnOffCoroutine != null)
            {
                StopCoroutine(_turnOnOffCoroutine);
            }
            _turnOnOffCoroutine = StartCoroutine(TurnOnOffCoroutine(on));
        }

        private IEnumerator TurnOnOffCoroutine(bool on)
        {
            AnimationCurve curve = on ? _turnOnCurve : _turnOffCurve;
            yield return Utilities.LerpOverTime(0f, 1f, curve.GetCurveDuration(), SetExpansionAmount, curve);
            _overlayIsOn = on;
        }

        /// <summary>
        /// Sets 
        /// </summary>
        /// <param name="value"></param>
        private void SetExpansionAmount(float value)
        {
            _material.SetFloat(Expansion, value * OverlayOnExpansionValue);
        }
    }
}