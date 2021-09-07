﻿using System;
using System.Collections;
using Code.Level;
using UnityEngine;

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
        
        public void ShowInterLevelUI(Action onShown = null, InterLevelTransition instant = InterLevelTransition.Regular, bool isFirstPerfect = false, bool showAdvanceLevelPrompt = false)
        {
            if (_showHideInterLevelCoroutine != null)
            {
                StopCoroutine(_showHideInterLevelCoroutine);
            }
            _showHideInterLevelCoroutine = StartCoroutine(ShowInterLevelUICoroutine(onShown, instant, isFirstPerfect, showAdvanceLevelPrompt));
        }

        public void HideInterLevelUI()
        {
            if (_showHideInterLevelCoroutine != null)
            {
                StopCoroutine(_showHideInterLevelCoroutine);
            } 
            _showHideInterLevelCoroutine = StartCoroutine(HideInterLevelUICoroutine());
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
        
        private IEnumerator ShowInterLevelUICoroutine(Action onShown, InterLevelTransition transition, bool isFirstPerfect, bool showAdvanceLevelPrompt)
        {
            _isTransitioning = true;
            if (transition == InterLevelTransition.Regular)
            {
                yield return new WaitForSeconds(_startDelay);
            }

            _interLevelScreen.SetupInterLevelScreen(isFirstPerfect, showAdvanceLevelPrompt);
            
            // Show overlay, hides level reset
            yield return ShowOverlayCoroutine(transition);
            
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

        private IEnumerator ShowOverlayCoroutine(InterLevelTransition transition)
        {
            LevelInstanceBase currentLevel = _levelManager.CurrentLevel;
            
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