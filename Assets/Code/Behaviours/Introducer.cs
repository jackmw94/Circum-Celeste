using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtras.Code.Core;

namespace Code.Behaviours
{
    public class Introducer : MonoBehaviour, IBeginDragHandler, ISelectHandler
    {
        [SerializeField] private Behaviour[] _introductionBehaviour;
        [Space(15)]
        [SerializeField] private bool _stopOnSelect = true;
        [SerializeField] private bool _stopOnDrag = true;
        private void Start()
        {
            _introductionBehaviour.ApplyFunction(p => p.enabled = false);
        }
        
        public void SetIntroducing(bool isIntroducing)
        {
            _introductionBehaviour.ApplyFunction(p => p.enabled = isIntroducing);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_stopOnDrag)
            {
                return;
            }
            
            SetIntroducing(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!_stopOnSelect)
            {
                return;
            }
            
            SetIntroducing(false);
        }
    }
}