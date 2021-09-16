using UnityEngine;

namespace Code.UI
{
    public class ScrollRectContentResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;
        [Space(15)]
        [SerializeField] private bool _isVertical = true;
        [SerializeField] private int _screenCount = 2;
        
        private void Start()
        {
            ResetSize();
        }

        [ContextMenu(nameof(ResetSize))]
        private void ResetSize()
        {
            Vector2 viewportSizeDelta = _viewport.rect.size;
            Vector2 contentSize = _isVertical ? viewportSizeDelta.y * _screenCount * Vector2.up : viewportSizeDelta.x * _screenCount * Vector2.right; 
            _content.sizeDelta = contentSize;
        }
    }
}