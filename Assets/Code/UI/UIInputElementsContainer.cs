using Code.Behaviours;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class UIInputElementsContainer : MonoBehaviour
    {
        [SerializeField] private MoverHandle _moverHandle;
        [SerializeField] private PulseColour _pulseMoverHandle;
        
        [SerializeField] private MomentaryButton _powerButton;
        [SerializeField] private PulseColour _pulsePowerButton;

        public MoverHandle MoverHandle => _moverHandle;
        public PulseColour PulseMoverHandle => _pulseMoverHandle;
        
        public MomentaryButton PowerButton => _powerButton;
        public PulseColour PulsePowerButton => _pulsePowerButton;
    }

    public class LevelScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelNameText;
    }
}