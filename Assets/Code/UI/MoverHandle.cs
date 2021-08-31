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
        [SerializeField] private float _relativeMovementSensitivity = 0.1f;
        [SerializeField] private float _returnSpeedMultiplier = 1f;
        [SerializeField] private AnimationCurve _returnSpeed;
        [SerializeField] private bool _useSquaredMovement = true;
        [SerializeField] private bool _useRelativeMovement = false;
        [SerializeField] private float _deadZone = 0.1f;

        private Vector2 _movement;
        private Vector2 _dragPosition;
        private Vector2 _dragDelta;
        private int _dragDeltaFrame;
        private Vector2 _centre;
        private bool _isDragging;

        public Vector2 Movement => _useSquaredMovement ? SquaredMovement : _movement;
        private Vector2 SquaredMovement => _movement.normalized * Mathf.Pow(_movement.magnitude, 2f);
        
        // OnDrag is not called every frame between drag starting and finishing
        // This will ensure that we only have a drag delta value when it's been set this frame
        private Vector2 DragDelta => Time.frameCount == _dragDeltaFrame ? _dragDelta : Vector2.zero;

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
            if (_isDragging)
            {
                if (_useRelativeMovement)
                {
                    HandleRelativeDragUpdate();
                }
                else
                {
                    HandleDragUpdate();
                }
            }
            else
            {
                HandleIdleUpdate();
            }
        }

        private void HandleDragUpdate()
        {
            UpdateHandlePosition();
            
            Vector2 offset = _dragPosition - _centre;
            _movement = offset / _maxRadius;
            
            _movement = GetDeadZoneAdjustedMovement(_movement);
        }

        private void HandleRelativeDragUpdate()
        {
            UpdateHandlePosition();

            Vector2 offset = DragDelta * _relativeMovementSensitivity;
            if (offset.magnitude > 1f)
            {
                offset = offset.normalized;
            }
            _movement = offset;
        }

        private void UpdateHandlePosition()
        {
            Transform handleTransform = transform;
            
            Vector2 offset = _dragPosition - _centre;
            if (offset.magnitude > _maxRadius)
            {
                offset = offset.normalized * _maxRadius;
            }

            handleTransform.localPosition = _centre + offset;
        }

        private void HandleIdleUpdate()
        {
            Transform moverTransform = transform;
            _movement = Vector2.zero;
            
            Vector3 position = moverTransform.localPosition;
            Vector2 centreOffset = _centre - new Vector2(position.x, position.y);
            float magnitude = centreOffset.magnitude;

            if (magnitude <= 0.001f)
            {
                // already at centre
                return;
            }
            
            float normalisedMagnitude = magnitude / _maxRadius;
            float returnDistance = _returnSpeed.Evaluate(normalisedMagnitude) * _returnSpeedMultiplier;
            Vector2 returnVector = centreOffset;
                
            if (magnitude > returnDistance)
            {
                returnVector = returnVector.normalized * returnDistance;
            }
            moverTransform.localPosition += new Vector3(returnVector.x, returnVector.y, 0f);
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
            Vector2 previousDragPosition = _dragPosition;
            
            Vector3 screenToWorldPoint = _camera.ScreenToWorldPoint(eventData.position);
            _dragPosition = transform.parent.InverseTransformPoint(screenToWorldPoint);
            
            _dragDelta = _dragPosition - previousDragPosition;
            _dragDeltaFrame = Time.frameCount;
        }

        private void UpdateConfigurableValues()
        {
            _useRelativeMovement = RemoteConfigHelper.MoverUIRelative;
            _deadZone = RemoteConfigHelper.MoverDeadZone;
            _relativeMovementSensitivity = RemoteConfigHelper.MoverUIRelativeMovementSensitivity;
        }
    }
}