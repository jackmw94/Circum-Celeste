using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.UI
{
    public class UIInputElementsContainer : SingletonMonoBehaviour<UIInputElementsContainer>
    {
        [SerializeField] private MoverHandle _moverHandle;
        [SerializeField] private MomentaryButton _slingButton;

        public MoverHandle MoverHandle => _moverHandle;
        public MomentaryButton SlingButton => _slingButton;
    }
}