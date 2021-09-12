using System.Collections;
using Code.Behaviours;
using Code.Core;
using Code.Debugging;
using Code.Level.Player;
using Code.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Code.Flow
{
    public class SplashScreen : MonoBehaviour
    {
        public const string UserHasSeenCircumLogoPlayerPrefsKey = "Circum_PlayerSeenFullLogos";
        private const int IntroSceneIndex = 0;
        private const int GameSceneIndex = 1;
        
        [SerializeField] private Animation _jonkWongleLogoAnimator;
        [SerializeField] private float _startJonkWonglerLogoDelay = 0.75f;
        [SerializeField] private float _playCircumLogoAtNormalisedTimeInJonkWonglerLogo = 0.65f;
        [SerializeField] private float _showGameAtNormalisedTimeInCircumLogo = 0.9f;
        [SerializeField] private VideoPlayer _circumLogo;
        [SerializeField] private AnimateImageShaderProperty _hideSplashScreen;

        private bool _remoteConfigReturned = false;
        private bool _loadedGameScene = false;

        private bool PlayerHasSeenFullLogos => CircumPlayerPrefs.HasKey(UserHasSeenCircumLogoPlayerPrefsKey);

        private void Awake()
        {
            _circumLogo.frame = 0;
            RemoteConfigHelper.RequestRefresh(success =>
            {
                _remoteConfigReturned = true;
            });

            StartCoroutine(PlayJonkWongleLogo(PlayerHasSeenFullLogos));
            
            SceneManager.LoadSceneAsync(GameSceneIndex, LoadSceneMode.Additive).completed += operation =>
            {
                _loadedGameScene = true;
            };
        }

        private IEnumerator Start()
        {
            _circumLogo.Play();
            CircumDebug.Log("Playing circum logo for warm up");
            yield return new WaitForSeconds(0.5f);
            CircumDebug.Log("Stopping circum logo for warm up");
            _circumLogo.Stop();
        }

        private IEnumerator PlayJonkWongleLogo(bool showCircumLogoToo)
        {
            yield return new WaitForSeconds(_startJonkWonglerLogoDelay);
            _jonkWongleLogoAnimator.Play();
            yield return showCircumLogoToo ? HandlePlayBothLogos() : HandleJustPlayJonkWongleLogo();
        }

        private IEnumerator CompleteSplashScreen()
        {
            if (!_remoteConfigReturned)
            {
                Popup.Instance.EnqueueMessage("Could not update game configuration!");
            }

            CircumDebug.Log("Waiting until game scene loaded");
            yield return new WaitUntil(() => _loadedGameScene);
            CircumDebug.Log("Wait for game scene to be loaded COMPLETE");

            CircumPlayerPrefs.SetInt(UserHasSeenCircumLogoPlayerPrefsKey, 1);
            CircumPlayerPrefs.Save();
            HideSplash();
        }

        private void HideSplash()
        {
            CircumDebug.Log("Triggering split animation");
            _hideSplashScreen.TriggerAnimation(() =>
            {
                CircumDebug.Log("Unloading splash screen");
                SceneManager.UnloadSceneAsync(IntroSceneIndex);
            });
        }

        private IEnumerator HandleJustPlayJonkWongleLogo()
        {
            yield return WaitUntilJonkWongleLogoFinished(false);
            yield return CompleteSplashScreen();
        }

        private IEnumerator HandlePlayBothLogos()
        {
            yield return WaitUntilJonkWongleLogoFinished(true);
            _circumLogo.Play();
            yield return WaitUntilCircumLogoFinished(false);
            yield return CompleteSplashScreen();
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
        
        private IEnumerator WaitUntilCircumLogoFinished(bool atReducedTime)
        {
            double requiredTime = (atReducedTime ? _showGameAtNormalisedTimeInCircumLogo : 0.99f) * _circumLogo.clip.length;
            double previousFrameTime = _circumLogo.time;
            
            yield return new WaitUntil(() =>
            {
                if (_circumLogo.time >= requiredTime) return true;
                if (previousFrameTime > _circumLogo.time) return true;

                previousFrameTime = _circumLogo.time;
                return false;
            });
            
            CircumDebug.Log("Circum logo finished");
        }
    }
}