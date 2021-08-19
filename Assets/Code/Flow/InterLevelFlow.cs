using System.Collections;
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
        [SerializeField] private float _delay = 3f;
        
        public bool ShowOverlayInstant { get; set; }
        
        protected override IEnumerator ActionStarted()
        {
            // Show overlay to hide level reset
            yield return ShowOverlay();
            
            // This is where the behind the scenes resetting happens
            ActionCompleted();

            // Hide again after delay
            yield return new WaitForSeconds(_delay);
            yield return HideOverlay();
        }

        private IEnumerator ShowOverlay()
        {
            LevelInstance currentLevel = LevelManager.Instance.CurrentLevel;
            Vector3 playerPosition = currentLevel.GetPlayerPosition(0);
            _levelOverlay.ShowOverlay(playerPosition, ShowOverlayInstant);
            
            yield return new WaitUntil(() => _levelOverlay.OverlayIsOn);

            _levelText.text = LevelManager.Instance.GetNextLevelName();
            
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