using System;
using System.Collections;
using Code.Level;
using UnityEngine;

namespace Code.Flow
{
    public class InterLevelFlow : MonoBehaviour
    {
        [SerializeField] private InterLevelScreen _interLevelScreen;
        [SerializeField] private LevelOverlay _levelOverlay;
        [SerializeField] private LevelManager _levelManager;
        [Space(15)]
        [SerializeField] private float _showHideHoldDelay = 0.25f;
        [SerializeField] private float _startDelay = 1.5f;

        private bool _isTransitioning = false;
        private Coroutine _showHideInterLevelCoroutine = null;

        public bool IsOverlaid => _levelOverlay.OverlayIsOn;
        public bool IsTransitioning => _isTransitioning;
        
        public void ShowInterLevelUI(Action onShown = null, bool instant = false)
        {
            if (_showHideInterLevelCoroutine != null)
            {
                StopCoroutine(_showHideInterLevelCoroutine);
            }
            _showHideInterLevelCoroutine = StartCoroutine(ShowInterLevelUICoroutine(onShown, instant));
        }

        public void HideInterLevelUI()
        {
            if (_showHideInterLevelCoroutine != null)
            {
                StopCoroutine(_showHideInterLevelCoroutine);
            } 
            _showHideInterLevelCoroutine = StartCoroutine(HideInterLevelUICoroutine());
        }

        public void ShowHideUI(Action onShown)
        {
            if (_showHideInterLevelCoroutine != null)
            {
                StopCoroutine(_showHideInterLevelCoroutine);
            }

            _showHideInterLevelCoroutine = StartCoroutine(ShowHideUICoroutine(onShown));
        }

        private IEnumerator ShowHideUICoroutine(Action onShown)
        {
            _isTransitioning = true;
            yield return ShowOverlayCoroutine(false);
            onShown();
            yield return new WaitForSeconds(_showHideHoldDelay);
            yield return HideOverlayCoroutine(false);
            _isTransitioning = false;

        }
        
        private IEnumerator ShowInterLevelUICoroutine(Action onShown, bool instant)
        {
            _isTransitioning = true;
            if (!instant)
            {
                yield return new WaitForSeconds(_startDelay);
            }

            _interLevelScreen.SetupInterLevelScreen();
            
            // Show overlay, hides level reset
            yield return ShowOverlayCoroutine(instant);
            
            yield return _interLevelScreen.ShowHideScreen(true);

            onShown?.Invoke();
            _isTransitioning = false;
        }

        private IEnumerator HideInterLevelUICoroutine()
        {
            _isTransitioning = true;
            yield return _interLevelScreen.ShowHideScreen(false);
            yield return HideOverlayCoroutine(false);
            _isTransitioning = false;
        }

        private IEnumerator ShowOverlayCoroutine(bool instant)
        {
            LevelInstance currentLevel = _levelManager.CurrentLevel;
            
            Vector3 playerPosition = Vector3.one / 2f;
            if (currentLevel)
            {
                // if there is no current level then this is probably at the start of the game
                // at this point we're calling with ShowOverlayInstant so position doesn't matter
                playerPosition = currentLevel.GetPlayerPosition(0);
            }

            _levelOverlay.ShowOverlay(playerPosition, instant);

            yield return new WaitUntil(() => _levelOverlay.OverlayIsOn);
        }

        private IEnumerator HideOverlayCoroutine(bool instant)
        {
            _levelOverlay.HideOverlay(instant);
            yield return new WaitUntil(() => !_levelOverlay.OverlayIsOn);
        }
    }
}