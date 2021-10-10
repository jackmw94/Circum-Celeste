using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class BadgesHintButton : MonoBehaviour
    {
        [SerializeField] private int _badgesHintIndex;
        [SerializeField] private ScrollingItemPicker _scrollingItemPicker;
        [SerializeField] private CircumScreen _circumTips;
        [SerializeField] private Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(BadgesHintButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(BadgesHintButtonClicked);
        }

        private void BadgesHintButtonClicked()
        {
            _circumTips.ShowHideScreen(true);
            _scrollingItemPicker.SetToItemAtIndex(_badgesHintIndex);
        }
    }
}