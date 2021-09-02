using UnityEngine;
using UnityEngine.UI;

namespace Code.Behaviours
{
    public class AnimateWithScrollRect : MonoBehaviour
    {
        private const float MinScrollValue = -0.5f;
        private const float MaxScrollValue = 1.5f;

        [SerializeField] private bool _invert;
        [SerializeField] private string _animatorProperty = "NormalisedTime";
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Animator _animator;

        private int _animatorPropertyHash;

        private void Awake()
        {
            _animatorPropertyHash = Animator.StringToHash(_animatorProperty);
        }

        private void Update()
        {
            float clampedNormalisedPosition = Mathf.Clamp(_scrollRect.verticalNormalizedPosition, MinScrollValue, MaxScrollValue);
            float remappedNormalisedPosition = Mathf.InverseLerp(MinScrollValue, MaxScrollValue, clampedNormalisedPosition);
            
            // would get weird looping behaviours when setting animation normalised time around 0 or 1
            float reducedNormalisedPosition = Mathf.Lerp(0.025f, 0.975f, remappedNormalisedPosition);
            
            reducedNormalisedPosition = _invert ? 1f - reducedNormalisedPosition : reducedNormalisedPosition;
            
            _animator.SetFloat(_animatorPropertyHash, reducedNormalisedPosition);
        }
    }
}