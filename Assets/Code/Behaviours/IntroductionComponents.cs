using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Behaviours
{
    public class IntroductionComponents : IntroductionBehaviour
    {
        [SerializeField] private Behaviour[] _introductionComponents;
        
        private void OnEnable()
        {
            _introductionComponents.ApplyFunction(p => p.enabled = true);
        }
        
        private void OnDisable()
        {
            _introductionComponents.ApplyFunction(p => p.enabled = false);
        }
    }
}