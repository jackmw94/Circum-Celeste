using Code.Level.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class LevelOverviewArrowButtons : MonoBehaviour
    {
        [SerializeField] private Button _leftArrowButton;
        [SerializeField] private Button _rightArrowButton;
        [Space(15)]
        [SerializeField] private Color _regularColour;
        [SerializeField] private Color _beforeClickedColour;

        private void Awake()
        {
            SetButtonColour();

            _leftArrowButton.onClick.AddListener(ArrowButtonClicked);
            _rightArrowButton.onClick.AddListener(ArrowButtonClicked);
        }

        private void OnDestroy()
        {
            _leftArrowButton.onClick.RemoveListener(ArrowButtonClicked);
            _rightArrowButton.onClick.RemoveListener(ArrowButtonClicked);
        }

        private void SetButtonColour()
        {
            PlayerFirsts playerFirsts = PersistentDataManager.Instance.PlayerFirsts;
            bool usedLevelOverviewArrows = playerFirsts.UsedLevelOverviewArrows;
            
            Color buttonColour = usedLevelOverviewArrows ? _regularColour : _beforeClickedColour;
            _leftArrowButton.image.color = buttonColour;
            _rightArrowButton.image.color = buttonColour;
        }

        private void ArrowButtonClicked()
        {
            PersistentDataManager.Instance.PlayerFirsts.SetLevelOverviewArrowsUsed();
            SetButtonColour();
        }
    }
}