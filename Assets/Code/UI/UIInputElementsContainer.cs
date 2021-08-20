using Code.Behaviours;
using UnityEngine;

namespace Code.UI
{
    public class UIInputElementsContainer : MonoBehaviour
    {
        [SerializeField] private MoverHandle _moverHandle;
        [SerializeField] private MomentaryButton _slingButton;
        [SerializeField] private PulseButton _pulseButton;

        public MoverHandle MoverHandle => _moverHandle;
        public MomentaryButton SlingButton => _slingButton;
        public PulseButton PulseButton => _pulseButton;
    }
}