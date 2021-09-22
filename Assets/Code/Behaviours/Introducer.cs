using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtras.Code.Core;

namespace Code.Behaviours
{
    public class Introducer : MonoBehaviour, ISelectHandler, IDragHandler
    {
        [SerializeField] private IntroductionBehaviour[] _introductionBehaviour;
        [Space(15)]
        [SerializeField] private bool _stopOnSelect = true;
        [Space(15)]
        [SerializeField] private bool _stopOnDrag = true;
        [SerializeField] private float _stopOnDragDurationThreshold = 0.75f;

        private float _currentDragTime = 0f;
        
        private void Start()
        {
            _introductionBehaviour.ApplyFunction(p => p.enabled = false);
        }
        
        public void SetIntroducing(bool isIntroducing)
        {
            _currentDragTime = 0f;
            _introductionBehaviour.ApplyFunction(p => p.enabled = isIntroducing);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!_stopOnSelect)
            {
                return;
            }
            
            SetIntroducing(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_stopOnDrag)
            {
                return;
            }

            // todo: test whether input system events are called at same rate as update
            _currentDragTime += Time.deltaTime;
            if (_currentDragTime > _stopOnDragDurationThreshold)
            {
                SetIntroducing(false);
            }
        }
    }
}