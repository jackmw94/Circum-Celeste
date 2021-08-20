using System;
using System.Collections;
using UnityEngine;

namespace Code.Flow
{
    public abstract class FlowBehaviour : MonoBehaviour
    {
        private Action _onComplete = null;
        
        public void StartAction(Action onComplete)
        {
            _onComplete = onComplete;
            StartCoroutine(ActionStarted());
        }

        protected abstract IEnumerator ActionStarted();

        protected void ActionCompleted()
        {
            _onComplete?.Invoke();
        }
    }
}
