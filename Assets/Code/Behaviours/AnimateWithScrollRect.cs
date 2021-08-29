using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Optional.EasingFunctions;

namespace Code.Behaviours
{
    public class AnimateWithScrollRect : MonoBehaviour
    {
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
            float clampedNormalisedPosition = Mathf.Clamp01(_scrollRect.verticalNormalizedPosition);
            float easedNormalisedPosition = EasingFunctions.EaseInOutSine(clampedNormalisedPosition);
            float reducedNormalisedPosition = Mathf.Lerp(0.05f, 0.95f, easedNormalisedPosition);
            reducedNormalisedPosition = _invert ? 1f - reducedNormalisedPosition : reducedNormalisedPosition;
            
            _animator.SetFloat(_animatorPropertyHash, reducedNormalisedPosition);
        }
    }
}