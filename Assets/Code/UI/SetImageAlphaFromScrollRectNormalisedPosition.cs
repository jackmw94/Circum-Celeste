using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Core;

namespace Code.UI
{
    public class SetImageAlphaFromScrollRectNormalisedPosition : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Image _image;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private bool _isVertical;

        private void Update()
        {
            float normalisedPosition = _isVertical ? _scrollRect.normalizedPosition.y : _scrollRect.normalizedPosition.x;
            float alphaValue = _animationCurve.Evaluate(normalisedPosition);
            _image.SetImageAlpha(alphaValue);
        }
    }
}