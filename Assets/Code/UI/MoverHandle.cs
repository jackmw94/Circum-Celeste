using System.Diagnostics;
using Code.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.UI
{
    public class MoverHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _maxRadius;
        [SerializeField] private float _returnSpeedMultiplier = 1f;
        [SerializeField] private AnimationCurve _returnSpeed;
        [SerializeField] private bool _useSquaredMovement = true;
        [SerializeField] private bool _useRelativeMovement = false;
        [SerializeField] private float _relativeMovementThreshold = 0.1f;
        [SerializeField] private float _deadZone = 0.1f;

        private Vector2 _movement;
        private Vector2 _dragPosition;
        private Vector2 _centre;
        private bool _isDragging;

        public Vector2 Movement => _useSquaredMovement ? SquaredMovement : _movement;
        private Vector2 SquaredMovement => _movement.normalized * Mathf.Pow(_movement.magnitude, 2f);

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_camera) _camera = Camera.main;
        }

        private void Start()
        {
            _centre = transform.localPosition;
            RemoteConfigHelper.RemoteConfigUpdated += UpdateConfigurableValues;
            
            UpdateConfigurableValues();
        }

        private void OnDestroy()
        {
            RemoteConfigHelper.RemoteConfigUpdated -= UpdateConfigurableValues;
        }

        private void LateUpdate()
        {
            Transform moverTransform = transform;

            if (_isDragging)
            {
                Vector2 offset = _dragPosition - _centre;
                bool atMaxRadius = false;
                if (offset.magnitude > _maxRadius)
                {
                    offset = offset.normalized * _maxRadius;
                    atMaxRadius = true;
                }

                Vector3 previousPosition = moverTransform.localPosition;
                moverTransform.localPosition = _centre + offset;

                if (_useRelativeMovement && !atMaxRadius)
                {
                    var frameDifference = moverTransform.localPosition - previousPosition;
                    if (frameDifference.magnitude > _relativeMovementThreshold)
                    {
                        frameDifference = frameDifference.normalized;
                    }

                    _movement = frameDifference / _relativeMovementThreshold;
                }
                else
                {
                    _movement = offset / _maxRadius;
                }
            }
            else
            {
                _movement = Vector2.zero;
            
                Vector3 position = moverTransform.localPosition;
                Vector2 centreOffset = _centre - new Vector2(position.x, position.y);
                float magnitude = centreOffset.magnitude;

                if (magnitude > 0.001f)
                {
                    float normalisedMagnitude = magnitude / _maxRadius;
                    float returnDistance = _returnSpeed.Evaluate(normalisedMagnitude) * _returnSpeedMultiplier;
                    Vector2 returnVector = centreOffset;
                
                    if (magnitude > returnDistance)
                    {
                        returnVector = returnVector.normalized * returnDistance;
                    }
                    moverTransform.localPosition += new Vector3(returnVector.x, returnVector.y, 0f);
                }
            }
            
            _movement = GetDeadZoneAdjustedMovement(_movement);
        }

        private Vector2 GetDeadZoneAdjustedMovement(Vector2 regularMovement)
        {
            float movementMagnitude = regularMovement.magnitude;
            float adjustedMagnitude = Mathf.InverseLerp(_deadZone, 1f, movementMagnitude);
            return regularMovement.normalized * adjustedMagnitude;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 screenToWorldPoint = _camera.ScreenToWorldPoint(eventData.position);
            _dragPosition = transform.parent.InverseTransformPoint(screenToWorldPoint);
        }

        private void UpdateConfigurableValues()
        {
            _useRelativeMovement = RemoteConfigHelper.MoverUIRelative;
            _deadZone = RemoteConfigHelper.MoverDeadZone;
        }
    }
}