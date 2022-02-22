using Code.Behaviours;
using UnityCommonFeatures;
using UnityEngine;

namespace Code.Level
{
    public class BackgroundOverlay : ActivateOverDurationBehaviour
    {
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private string _progressParameterName = "Progress";
        [Space(15)]
        [SerializeField] private Vector2 _minMaxProgressValues;
        [Space(15)]
        [SerializeField] private string _rotationParameterName;
        [SerializeField] private float _turnOnRotation = 15f;
        [SerializeField] private float _turnOffRotation = 195f;

        private int _progressParameterId;
        private int _rotationParameterId;

        protected override void Awake()
        {
            base.Awake();
            _meshCollider.enabled = false;
            _progressParameterId = Shader.PropertyToID(_progressParameterName);
            _rotationParameterId = Shader.PropertyToID(_rotationParameterName);
        }

        protected override void OnActivateDeactivateStarted()
        {
            base.OnActivateDeactivateStarted();
            bool isActivating = CurrentVisibleState == VisibleState.ChangingToVisible;
            float rotation = isActivating ? _turnOnRotation : _turnOffRotation;
            _renderer.material.SetFloat(_rotationParameterId, rotation);
            _meshCollider.enabled = isActivating;
        }

        protected override void SetActivatedAmount(float amount)
        {
            float progress = Mathf.Lerp(_minMaxProgressValues.x, _minMaxProgressValues.y, amount);
            _renderer.material.SetFloat(_progressParameterId, progress);
        }
    }
}