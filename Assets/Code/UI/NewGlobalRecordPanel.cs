using System.Collections;
using Code.Juice;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Core;

namespace Code.UI
{
    public class NewGlobalRecordPanel : UnityCommonFeatures.UIPanel
    {
        [SerializeField] private float _cameraShakeFactor = 0.5f;
        [SerializeField] private float _showForTime = 5f;
        [Space(15)]
        [SerializeField] private ScreenShaker _screenShake;
        [SerializeField] private Image _rainbowBackgroundImage;
        [SerializeField] private ParticleSystem _particleSystem;

        private readonly string _shaderAlphaParameterName = "_Alpha";
        private Coroutine _showHideCoroutine;

        protected override void OnPanelAlphaSet(float alpha)
        {
            _rainbowBackgroundImage.material.SetFloat(_shaderAlphaParameterName, alpha);
        }

        public static void ShowNewGlobalRecordFeedback()
        {
            GetPanel<NewGlobalRecordPanel>().TriggerNewGlobalRecordFeedback();
        }

        [ContextMenu(nameof(TriggerNewGlobalRecordFeedback))]
        private void TriggerNewGlobalRecordFeedback()
        {
            this.RestartCoroutine(ref _showHideCoroutine, ShowNewGlobalRecordFeedbackCoroutine());
        }

        private IEnumerator ShowNewGlobalRecordFeedbackCoroutine()
        {
            _screenShake.AddShake(_cameraShakeFactor);
            _particleSystem.Play();
            Show();
            yield return new WaitForSeconds(_showForTime);
            Hide();
        }
    }
}