using Code.Level;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtras.Core;

namespace Code.Behaviours
{
    public class Introducer : MonoBehaviour, ISelectHandler, IDragHandler
    {
        [SerializeField] private IntroductionBehaviour[] _introductionBehaviour;
        [Space(15)]
        [SerializeField] private bool _stopOnLevelStart = true;
        [SerializeField] private bool _stopOnLevelQuit = true;
        [Space(15)]
        [SerializeField] private bool _stopOnSelect = true;
        [Space(15)]
        [SerializeField] private bool _stopOnDrag = true;
        [SerializeField] private float _stopOnDragDurationThreshold = 0.75f;

        private float _currentDragTime = 0f;
        
        private void Start()
        {
            _introductionBehaviour.ApplyFunction(p => p.SetEnabled(false));

            LevelInstanceBase.LevelStarted += OnLevelStarted;
            LevelInstanceBase.LevelStopped += OnLevelStopped;
        }

        private void OnDestroy()
        {
            LevelInstanceBase.LevelStarted -= OnLevelStarted;
            LevelInstanceBase.LevelStopped -= OnLevelStopped;
        }

        public void SetIntroducing(bool isIntroducing)
        {
            _currentDragTime = 0f;
            _introductionBehaviour.ApplyFunction(p => p.SetEnabled(isIntroducing));
        }

        private void OnLevelStarted()
        {
            if (!_stopOnLevelStart)
            {
                return;
            }
            
            SetIntroducing(false);
        }

        private void OnLevelStopped()
        {
            if (!_stopOnLevelQuit)
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