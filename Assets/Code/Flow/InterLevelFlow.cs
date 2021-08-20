using System.Collections;
using Code.Core;
using Code.Level;
using TMPro;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class InterLevelFlow : FlowBehaviour
    {
        [SerializeField] private LevelOverlay _levelOverlay;
        [SerializeField] private CanvasGroup _levelTextCanvasGroup;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _startDelay = 1.5f;
        [SerializeField] private float _holdDelay = 2f;
        
        public bool ShowOverlayInstant { get; set; }
        public bool ShowNextLevelName { get; set; }
        
        protected override IEnumerator ActionStarted()
        {
            if (!ShowOverlayInstant)
            {
                yield return new WaitForSeconds(_startDelay);
            }
            
            // Show overlay to hide level reset
            yield return ShowOverlay();
            
            // This is where the behind the scenes resetting happens
            ActionCompleted();

            // Hide again after delay
            yield return new WaitForSeconds(_holdDelay);
            yield return HideOverlay();
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

            _levelText.text = ShowNextLevelName ? levelManager.GetNextLevelName() : levelManager.GetCurrentLevelName();
            
            _levelTextCanvasGroup.interactable = true;
            _levelTextCanvasGroup.blocksRaycasts = true;
            yield return Utilities.LerpOverTime(0f, 1f, _fadeDuration, f =>
            {
                _levelTextCanvasGroup.alpha = f;
            });
        }

        private IEnumerator HideOverlay()
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