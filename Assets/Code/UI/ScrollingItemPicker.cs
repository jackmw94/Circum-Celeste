using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    [DefaultExecutionOrder(-1)]
    public class ScrollingItemPicker : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] protected ScrollRect _scrollRect;
        [SerializeField] protected RectTransform _root;
        [Space(15)]
        [SerializeField] private Transform _selector;
        [SerializeField] private float _magnetismFactor = 0.05f;
        [Space(15)]
        [SerializeField] private float _turnOnDuration;
        [SerializeField] private float _turnOffDuration;
        [SerializeField] private Color _unselectedColour;
        [SerializeField] private Color _selectedColour;

        private Selectable[] _items = new Selectable[0];
        private int _currentlySelectedIndex = -1;
        private bool _isScrollRectInteractedWith = false;

        public int NumberOfItems => _items.Length;
        public int CurrentlySelectedIndex => _currentlySelectedIndex;
        public Selectable CurrentlySelected => _items[_currentlySelectedIndex];

        private void Awake()
        {
            _items = GetScrollItems().OrderBy(p => p.transform.GetSiblingIndex()).ToArray();
        }

        protected virtual Selectable[] GetScrollItems()
        {
            return GetComponentsInChildren<Selectable>();
        }

        private void Update()
        {
            if (_root.childCount == 0)
            {
                return;
            }
        
            DetermineSelectedItem();
            HandleMagnetism();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isScrollRectInteractedWith = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isScrollRectInteractedWith = false;
        }

        public void SetToItemAtIndex(int index)
        {
            Selectable selectedItem = _items[index];
            selectedItem.SetOnOff(true, _selectedColour, 0f);

            int itemCount = _items.Length;
            int divisor = itemCount - 1;
            float perItemFraction = 1f / divisor;
            float normalisedScrollPosition = 1f - perItemFraction * index;
        
            _scrollRect.verticalNormalizedPosition = normalisedScrollPosition;
        }

        private void DetermineSelectedItem()
        {
            float selectorYPosition = _selector.position.y;
            float minimumDistanceFromSelector = float.MaxValue;
            _currentlySelectedIndex = -1;

            for (int i = 0; i < _root.childCount; i++)
            {
                Transform childTransform = _root.GetChild(i);
                float itemDistance = Mathf.Abs(childTransform.position.y - selectorYPosition);
                if (itemDistance < minimumDistanceFromSelector)
                {
                    minimumDistanceFromSelector = itemDistance;
                    _currentlySelectedIndex = i;
                }
            }

            if (_currentlySelectedIndex == -1)
            {
                Debug.LogError($"Could not get a selected item despite there being {_root.childCount} items");
                return;
            }
            
            foreach (Selectable scrollItem in _items)
            {
                if (scrollItem == CurrentlySelected)
                {
                    scrollItem.SetOnOff(true, _selectedColour, _turnOnDuration);
                }
                else
                {
                    scrollItem.SetOnOff(false, _unselectedColour, _turnOffDuration);
                }
            }
        }

        private void HandleMagnetism()
        {
            if (_isScrollRectInteractedWith || _root.sizeDelta.y < float.Epsilon)
            {
                return;
            }

            Transform selectedItemTransform = _root.GetChild(_currentlySelectedIndex);
            float distanceToSelector = _selector.transform.position.y - selectedItemTransform.position.y;
            float fractionalDistance = distanceToSelector / _root.sizeDelta.y;
            float moveDistance = _magnetismFactor * fractionalDistance;
            _scrollRect.verticalNormalizedPosition -= moveDistance;
        }

        [ContextMenu(nameof(NameObjectsFromLabels))]
        private void NameObjectsFromLabels()
        {
            foreach (Transform child in _root)
            {
                TextMeshProUGUI label = child.GetComponentInChildren<TextMeshProUGUI>();
                child.gameObject.name = label.text;
            }
        }
    }
}