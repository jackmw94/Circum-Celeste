using System;
using System.Collections;
using Code.Debugging;
using Code.Level;
using Code.Level.Player;
using Code.VFX;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Flow
{
    public class InterLevelFlow : MonoBehaviour
    {
        public enum InterLevelTransition
        {
            Regular,
            Fast,
            Instant
        }

        [SerializeField] private LevelProvider _levelProvider;
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

        private bool SwipeToChangeLevelAvailable => IsOverlaid && !_isTransitioning && !Settings.Instance.SettingsAreShowing;

        private void Start()
        {
            ShowInterLevelUI(transition: InterLevelTransition.Instant);
        }

        private void Update()
        {
            if (!SwipeToChangeLevelAvailable)
            {
                return;
            }
            
            CircumGestures.SwipeDirection swipeDirection = CircumGestures.GetSwipeDirection();
            switch (swipeDirection)
            {
                case CircumGestures.SwipeDirection.None:
                    break;
                case CircumGestures.SwipeDirection.Right:
                    if (_levelProvider.CanChangeToPreviousLevel())
                    {
                        _levelProvider.PreviousLevel();
                        _interLevelScreen.SetupInterLevelScreen();
                    }
                    break;
                case CircumGestures.SwipeDirection.Left:
                    if (_levelProvider.CanChangeToNextLevel(false))
                    {
                        _levelProvider.AdvanceLevel();
                        _interLevelScreen.SetupInterLevelScreen();
                    }
                    break;
            }
        }
        
        public void ShowInterLevelUI(Action onShown = null, InterLevelTransition transition = InterLevelTransition.Regular, BadgeData newBadgeData = new BadgeData(), NewFastestTimeInfo newFastestTimeInfo = null, bool hasComeFromLevelCompletion = false, bool firstTimeCompletingLevel = false)
        {
            if (_showHideInterLevelCoroutine != null)
            {
                StopCoroutine(_showHideInterLevelCoroutine);
            }
            _showHideInterLevelCoroutine = StartCoroutine(ShowInterLevelUICoroutine(onShown, transition, newBadgeData, newFastestTimeInfo, hasComeFromLevelCompletion, firstTimeCompletingLevel));
        }

        public void HideInterLevelUI(InterLevelTransition transition = InterLevelTransition.Regular)
        {
            if (_showHideInterLevelCoroutine != null)
            {
                StopCoroutine(_showHideInterLevelCoroutine);
            } 
            _showHideInterLevelCoroutine = StartCoroutine(HideInterLevelUICoroutine(transition));
        }

        public void ShowHideUI(Action onShown, InterLevelTransition transition)
        {
            if (_showHideInterLevelCoroutine != null)
            {
                StopCoroutine(_showHideInterLevelCoroutine);
            }

            _showHideInterLevelCoroutine = StartCoroutine(ShowHideUICoroutine(onShown, transition));
        }

        private IEnumerator ShowHideUICoroutine(Action onShown, InterLevelTransition transition)
        {
            _isTransitioning = true;
            yield return ShowOverlayCoroutine(transition);
            onShown();
            yield return new WaitForSeconds(transition == InterLevelTransition.Regular ? _showHideHoldDelay : 0f);
            yield return HideOverlayCoroutine(false);
            _isTransitioning = false;

        }
        
        private IEnumerator ShowInterLevelUICoroutine(Action onShown, InterLevelTransition transition, BadgeData newBadgeData, NewFastestTimeInfo newFastestTimeInfo, bool hasComeFromLevelCompletion, bool firstTimeCompletingLevel)
        {
            _isTransitioning = true;
            if (transition == InterLevelTransition.Regular)
            {
                yield return new WaitForSeconds(_startDelay);
            }

            _interLevelScreen.SetupInterLevelScreen(newBadgeData, newFastestTimeInfo, hasComeFromLevelCompletion);
            
            // Show overlay, hides level reset
            yield return ShowOverlayCoroutine(transition);

            if (firstTimeCompletingLevel)
            {
                AppFeedbacks.Instance.TriggerComets();
            }
            
            yield return _interLevelScreen.ShowHideScreen(true, transition);

            onShown?.Invoke();
            _isTransitioning = false;
        }

        private IEnumerator HideInterLevelUICoroutine(InterLevelTransition transition)
        {
            _isTransitioning = true;
            yield return _interLevelScreen.ShowHideScreen(false, transition);
            yield return HideOverlayCoroutine(false);
            _isTransitioning = false;
        }

        private IEnumerator ShowOverlayCoroutine(InterLevelTransition transition)
        {
            LevelInstanceBase currentLevel = _levelManager.CurrentLevelInstance;
            
            Vector3 playerPosition = Vector3.one / 2f;
            if (currentLevel)
            {
                // if there is no current level then this is probably at the start of the game
                // at this point we're calling with ShowOverlayInstant so position doesn't matter
                playerPosition = currentLevel.GetPlayerPosition(0);
            }

            _levelOverlay.ShowOverlay(playerPosition, transition == InterLevelTransition.Instant);

            yield return new WaitUntil(() => _levelOverlay.OverlayIsOn);
        }

        private IEnumerator HideOverlayCoroutine(bool instant)
        {
            _levelOverlay.HideOverlay(instant);
            yield return new WaitUntil(() => !_levelOverlay.OverlayIsOn);
        }
    }
}