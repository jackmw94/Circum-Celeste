using UnityEngine;

namespace Code.UI
{
    public class InterLevelContentResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;
        [Space(15)]
        [SerializeField] private int _screenCount = 2;
        
        private void Start()
        {
            ResetSize();
        }

        [ContextMenu(nameof(ResetSize))]
        private void ResetSize()
        {
            Vector2 viewportSizeDelta = _viewport.rect.size;
            _content.sizeDelta = new Vector2(0f, viewportSizeDelta.y * _screenCount);
        }
    }
}