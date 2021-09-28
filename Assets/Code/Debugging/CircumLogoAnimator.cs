using System.Collections;
using Code.Behaviours;
using UnityEngine;

namespace Code.Debugging
{
    public class CircumLogoAnimator : MonoBehaviour
    {
        [SerializeField] private AnimateShaderPropertyBase _showLogo;
        [SerializeField] private AnimateShaderPropertyBase _hideLogo;
        [SerializeField] private AnimateShaderPropertyBase _hideBackground;
        [Space(15)]
        [SerializeField] private float _hideLogoDelay;
        
        private void OnEnable()
        {
            StartCoroutine(RunCircumLogoAnimation());
        }

        private IEnumerator RunCircumLogoAnimation()
        {
            bool logoShown = false;
            _showLogo.TriggerAnimation(() =>
            {
                logoShown = true;
            });

            yield return new WaitUntil(() => logoShown);
            
            CircumDebug.Log("Circum logo shown");

            yield return new WaitForSeconds(_hideLogoDelay);
            
            _hideLogo.TriggerAnimation();
            _hideBackground.TriggerAnimation();
        }
    }
}