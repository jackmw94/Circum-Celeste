using System;
using System.Collections;
using Code.Behaviours;
using Code.Core;
using Code.Debugging;
using Code.Level.Player;
using Code.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class SplashScreen : MonoBehaviour
    {
        private const float RefreshRemoteConfigTimeout = 2.5f;
        private const int DontShowSplashScreenAgainForTimeInMinutes = 30;
        private const int IntroSceneIndex = 0;
        private const int GameSceneIndex = 1;

        [SerializeField] private Volume _postProcessingVolume;
        [SerializeField] private Animation _jonkWongleLogoAnimator;
        [SerializeField] private float _startJonkWonglerLogoDelay = 0.75f;
        [SerializeField] private float _playCircumLogoAtNormalisedTimeInJonkWonglerLogo = 0.65f;
        [SerializeField] private float _holdSplashScreenDuration = 1.25f;
        [SerializeField] private AnimateMeshShaderProperty _showSplashScreen;
        [SerializeField] private AnimateMeshShaderProperty _hideSplashScreen;
        [SerializeField] private AnimateMeshShaderProperty _hideCircumLogo;

        private bool _remoteConfigRefreshSuccessful = false;
        private bool _remoteConfigReturned = false;
        private bool _loadedGameScene = false;
        private bool _splashScreensTriggered = false;

        private void Awake()
        {
            RemoteConfigHelper.RequestRefresh(success =>
            {
                _remoteConfigReturned = true;
                _remoteConfigRefreshSuccessful = success;
            });

            SceneManager.LoadSceneAsync(GameSceneIndex, LoadSceneMode.Additive).completed += operation => { _loadedGameScene = true; };
            
            TriggerSplashScreens();
        }

        public void TriggerSplashScreens()
        {
            CircumDebug.Assert(!_splashScreensTriggered, "Wrote this assuming splash screens would only be triggered once but appears they're being triggered more. Test this code against this assumption");
            StartCoroutine(ShouldPlaySplashScreen() ? PlayLogos() : CompleteSplashScreen(false));
            _splashScreensTriggered = true;
        }

        private bool ShouldPlaySplashScreen()
        {
            if (!PersistentDataHelper.TryGetLong(PersistentDataKeys.SplashScreenLastRunTime, out long lastRunTicks))
            {
                return true;
            }
            
            DateTime lastRunDateTime = new DateTime(lastRunTicks);
            TimeSpan timeSpanSinceLastRun = DateTime.Now - lastRunDateTime;
            CircumDebug.Log($"There have been {timeSpanSinceLastRun.Minutes} minutes since we last saw the splash screen");
            return timeSpanSinceLastRun.Minutes > DontShowSplashScreenAgainForTimeInMinutes;
        }
        
        
        private IEnumerator PlayLogos()
        {
            yield return new WaitForSeconds(_startJonkWonglerLogoDelay);
            _jonkWongleLogoAnimator.Play();

            yield return WaitUntilJonkWongleLogoFinished(true);

            yield return WaitUntilLoggedIn();
            
            bool showSplashComplete = false;
            _showSplashScreen.TriggerAnimation(() =>
            {
                showSplashComplete = true;
            });
            yield return new WaitUntil(() => showSplashComplete);
            yield return new WaitForSeconds(_holdSplashScreenDuration);

            yield return CompleteSplashScreen(true);
        }

        private IEnumerator WaitUntilLoggedIn()
        {
            bool socialAuthenticationFinished = false;

            RemoteDataManager.Instance.LoginWithSocialAPI(_ =>
            {
                socialAuthenticationFinished = true;
            });

            yield return new WaitUntil(() => socialAuthenticationFinished);
        }

        private IEnumerator CompleteSplashScreen(bool playedLogos)
        {
            if (playedLogos)
            {
                PersistentDataHelper.SetLong(PersistentDataKeys.SplashScreenLastRunTime, DateTime.Now.Ticks);
            }
            else
            {
                yield return WaitUntilLoggedIn();
            }

            float startTime = Time.time;
            bool HasTimedOut() => Time.time - startTime > RefreshRemoteConfigTimeout;

            yield return Utilities.LerpOverTime(_postProcessingVolume.weight, 0f, 0.25f, f => _postProcessingVolume.weight = f);

            yield return new WaitUntil(() => _loadedGameScene && (_remoteConfigReturned || HasTimedOut()));
            
            if (!_remoteConfigReturned || !_remoteConfigRefreshSuccessful)
            {
                Popup.Instance.EnqueueMessage(Popup.LocalisedPopupType.CantRefreshConfig);
            }

            HideSplash(playedLogos);
        }

        private void HideSplash(bool playedLogos)
        {
            if (playedLogos)
            {
                _hideCircumLogo.TriggerAnimation();
            }
            
            _hideSplashScreen.TriggerAnimation(() =>
            {
                SceneManager.UnloadSceneAsync(IntroSceneIndex);
            });
        }

        private IEnumerator WaitUntilJonkWongleLogoFinished(bool atReducedTime)
        {
            AnimationState animationState = _jonkWongleLogoAnimator[_jonkWongleLogoAnimator.clip.name];
            float requiredTime = atReducedTime ? _playCircumLogoAtNormalisedTimeInJonkWonglerLogo : 0.99f;
            float previousFrameNormalisedTime = animationState.normalizedTime;
            
            yield return new WaitUntil(() =>
            {
                if (animationState.normalizedTime >= requiredTime) return true;
                if (previousFrameNormalisedTime > animationState.normalizedTime) return true;

                previousFrameNormalisedTime = animationState.normalizedTime;
                return false;
            });
        }
    }
}