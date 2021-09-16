using System.Collections;
using Code.Behaviours;
using Code.Core;
using Code.Level.Player;
using Code.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Flow
{
    public class SplashScreen : MonoBehaviour
    {
        private const int IntroSceneIndex = 0;
        private const int GameSceneIndex = 1;
        
        [SerializeField] private Animation _jonkWongleLogoAnimator;
        [SerializeField] private float _startJonkWonglerLogoDelay = 0.75f;
        [SerializeField] private float _playCircumLogoAtNormalisedTimeInJonkWonglerLogo = 0.65f;
        [SerializeField] private float _holdSplashScreenDuration = 1.25f;
        [SerializeField] private AnimateMeshShaderProperty _showSplashScreen;
        [SerializeField] private AnimateMeshShaderProperty _hideSplashScreen;
        [SerializeField] private AnimateMeshShaderProperty _hideCircumLogo;

        private bool _remoteConfigReturned = false;
        private bool _loadedGameScene = false;

        private void Awake()
        {
            RemoteConfigHelper.RequestRefresh(success =>
            {
                _remoteConfigReturned = true;
            });

            StartCoroutine(PlayJonkWongleLogo());
            
            SceneManager.LoadSceneAsync(GameSceneIndex, LoadSceneMode.Additive).completed += operation =>
            {
                _loadedGameScene = true;
            };
        }

        private IEnumerator PlayJonkWongleLogo()
        {
            yield return new WaitForSeconds(_startJonkWonglerLogoDelay);
            _jonkWongleLogoAnimator.Play();
            yield return HandlePlayBothLogos();
        }

        private IEnumerator CompleteSplashScreen()
        {
            if (!_remoteConfigReturned)
            {
                Popup.Instance.EnqueueMessage("Could not update game configuration!");
            }

            yield return new WaitUntil(() => _loadedGameScene);

            HideSplash();
        }

        private void HideSplash()
        {
            _hideCircumLogo.TriggerAnimation();
            _hideSplashScreen.TriggerAnimation(() =>
            {
                SceneManager.UnloadSceneAsync(IntroSceneIndex);
            });
        }

        private IEnumerator HandlePlayBothLogos()
        {
            yield return WaitUntilJonkWongleLogoFinished(true);

            bool showSplashComplete = false;
            _showSplashScreen.TriggerAnimation(() =>
            {
                showSplashComplete = true;
            });
            yield return new WaitUntil(() => showSplashComplete);
            yield return new WaitForSeconds(_holdSplashScreenDuration);

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
    }
}