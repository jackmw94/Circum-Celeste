using Code.Behaviours;
using UnityEngine;

namespace Code.UI
{
    public class UIInputElementsContainer : MonoBehaviour
    {
        [SerializeField] private MoverHandle _moverHandle;
        [SerializeField] private Introducer _introduceMoverHandle;
        
        [SerializeField] private MomentaryButton _powerButton;
        [SerializeField] private Introducer _introducePowerButton;

        public MoverHandle MoverHandle => _moverHandle;
        public Introducer IntroduceMoverHandle => _introduceMoverHandle;
        
        public MomentaryButton PowerButton => _powerButton;
        public Introducer IntroducePowerButton => _introducePowerButton;
    }
}