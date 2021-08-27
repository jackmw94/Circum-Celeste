using System.Collections;
using Code.Core;
using Code.Level;
using Code.Level.Player;
using Code.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class InterLevelFlow : FlowBehaviour
    {
        [SerializeField] private LevelOverlay _levelOverlay;
        [SerializeField] private CanvasGroup _levelTextCanvasGroup;
        [SerializeField] private ScrollingItemPicker _scrollingItemPicker;
        [SerializeField] private PlayerStatsManager _playerStatsManager;
        [SerializeField] private Button _continueButton;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _tagText;
        [Space(15)]
        [SerializeField] private GameObject _highestLevelRoot;
        [SerializeField] private TextMeshProUGUI _highestLevelText;
        [SerializeField] private GameObject _highestNoDeathsRoot;
        [SerializeField] private TextMeshProUGUI _highestNoDeathsText;
        [SerializeField] private GameObject _highestPerfectRoot;
        [SerializeField] private TextMeshProUGUI _highestPerfectText;
        [Space(15)]
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _startDelay = 1.5f;
        [SerializeField] private float _holdDelay = 2f;
        
        public bool ShowOverlayInstant { get; set; }
        public bool ShowNextLevelName { get; set; }
        public bool PreventHidingOverlay { get; set; } = false;

        public bool IsOverlaid => _levelOverlay.OverlayIsOn;
        
        protected override IEnumerator ActionStarted()
        {
            if (!ShowOverlayInstant)
            {
                yield return new WaitForSeconds(_startDelay);
            }
            
            // Show overlay to hide level reset
            yield return ShowOverlay();
            yield return new WaitUntil(() => !PreventHidingOverlay);

            // This is where the behind the scenes resetting happens
            ActionCompleted();

            // Wait until button pressed
            bool buttonPressed = false;
            _continueButton.onClick.AddListener(() =>
            {
                buttonPressed = true;
            });
            yield return new WaitUntil(() => buttonPressed);
            _continueButton.onClick.RemoveAllListeners();

            // Hide both UI + overlay
            yield return HideAll();
        }

        private IEnumerator ShowOverlay()
        {
            GameContainer gameContainer = GameContainer.Instance;
            LevelManager levelManager = gameContainer.LevelManager;
            LevelInstance currentLevel = levelManager.CurrentLevel;
            
            Vector3 playerPosition = Vector3.one / 2f;
            if (currentLevel)
            {
                // if there is no current level then this is probably at the start of the game
                // at this point we're calling with ShowOverlayInstant so position doesn't matter
                playerPosition = currentLevel.GetPlayerPosition(0);
            }

            _levelOverlay.ShowOverlay(playerPosition, ShowOverlayInstant);

            yield return new WaitUntil(() => _levelOverlay.OverlayIsOn);

            // set scroll to bottom item
            _scrollingItemPicker.SetToItemAtIndex(_scrollingItemPicker.NumberOfItems - 1);
            
            LevelLayout showLevel = ShowNextLevelName ? levelManager.GetNextLevel() : levelManager.GetCurrentLevel();
            _levelText.text = showLevel.name;
            
            int levelIndex = showLevel.LevelContext.LevelNumber;
            _levelNumberText.text = $"{levelIndex}.";
            _levelNumberText.enabled = levelIndex > 0;
            
            _tagText.text = showLevel.TagLine;

            PlayerStats playerStats = _playerStatsManager.PlayerStats;

            _highestLevelRoot.SetActive(playerStats.HighestLevel > 0);
            _highestLevelText.text = playerStats.HighestLevel.ToString();

            _highestNoDeathsRoot.SetActive(playerStats.HighestLevelNoDeaths > 0);
            _highestNoDeathsText.text = playerStats.HighestLevelNoDeaths.ToString();

            _highestPerfectRoot.SetActive(playerStats.HighestPerfectLevel > 0);
            _highestPerfectText.text = playerStats.HighestPerfectLevel.ToString();
            
            _levelTextCanvasGroup.interactable = true;
            _levelTextCanvasGroup.blocksRaycasts = true;
            yield return Utilities.LerpOverTime(0f, 1f, _fadeDuration, f =>
            {
                _levelTextCanvasGroup.alpha = f;
            });
        }

        private IEnumerator HideAll()
        {
            yield return Utilities.LerpOverTime(1f, 0f, _fadeDuration, f =>
            {
                _levelTextCanvasGroup.alpha = f;
            });
            
            _levelOverlay.HideOverlay();
            yield return new WaitUntil(() => !_levelOverlay.OverlayIsOn);
            
            _levelTextCanvasGroup.interactable = false;
            _levelTextCanvasGroup.blocksRaycasts = false;
        }
    }
}