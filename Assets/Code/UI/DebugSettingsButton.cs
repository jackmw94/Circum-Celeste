using System.Collections;
using System.Linq;
using Code.Core;
using UnityCommonFeatures;
using UnityEngine;

namespace Code.UI
{
    public class DebugSettingsButton : ButtonBehaviour
    {
        [SerializeField] private GameObject _debugSettings;
        
        private void Awake()
        {
            _debugSettings.SetActive(false);
            
#if CIRCUM_DEBUG
            gameObject.SetActive(true);
#else
            gameObject.SetActive(false);
#endif
        }
        
        protected override void OnButtonClicked()
        {
            _debugSettings.SetActive(true);
        }
    }
}