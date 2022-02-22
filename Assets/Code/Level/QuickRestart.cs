using System.Collections.Generic;
using Code.Flow;
using UnityEngine;

namespace Code.Level
{
    public class QuickRestart : MonoBehaviour
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private float _tripleClickTimeThreshold = 0.5f;
        [SerializeField] private float _coolOff = 2f;

        private float _lastTriggeredTime = float.MinValue;
        private readonly Queue<float> _clickTimes = new Queue<float>();
        
        private void OnMouseDown()
        {
            if (Time.time - _lastTriggeredTime < _coolOff)
            {
                return;
            }
            
            UpdateClickTimesOnClick();

            if (HasBeenTriplePressed())
            {
                _lastTriggeredTime = Time.time;
                _clickTimes.Clear();
                _levelManager.CreateCurrentLevel(transition: InterLevelFlow.InterLevelTransition.Fast);
            }
        }

        private void UpdateClickTimesOnClick()
        {
            if (_clickTimes.Count >= 3)
            {
                _clickTimes.Dequeue();
            }

            _clickTimes.Enqueue(Time.time);
        }

        private bool HasBeenTriplePressed()
        {
            if (_clickTimes.Count < 3)
            {
                return false;
            }

            float firstPressTime = _clickTimes.Peek();
            float currentTime = Time.time;
            float timeDifference = currentTime - firstPressTime;
            return timeDifference < _tripleClickTimeThreshold;
        }
    }
}