using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class ScrollRectButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private ScrollingItemPicker _scrollingItemPicker;
        [SerializeField] private bool _increaseNormalisedPosition;

        private void Awake()
        {
            _button.onClick.AddListener(ButtonClickedListener);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ButtonClickedListener);
        }

        private void ButtonClickedListener()
        {
            int currentIndex = _scrollingItemPicker.CurrentlySelectedIndex;
            int nextIndex = currentIndex + (_increaseNormalisedPosition ? 1 : -1);
            nextIndex = Mathf.Clamp(nextIndex, 0, _scrollingItemPicker.NumberOfItems - 1);
            _scrollingItemPicker.SetToItemAtIndex(nextIndex);
        }
    }
}