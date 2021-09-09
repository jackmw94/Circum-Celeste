using System;
using System.Collections.Generic;
using Code.Debugging;
using Lean.Touch;
using UnityEngine;

namespace Code.Flow
{
    public static class CircumGestures
    {
        [Flags]
        public enum SwipeDirection
        {
            None = 0,
            Left = 1 << 0,
            Right = 1 << 1
        }
        
        private const float SwipeDirectionThreshold = 0.9f;
        
        private static readonly Dictionary<SwipeDirection, Vector2> _swipeDirectionVectors = new Dictionary<SwipeDirection, Vector2>()
        {
            {SwipeDirection.None, Vector2.zero},
            {SwipeDirection.Left, Vector2.left},
            {SwipeDirection.Right, Vector2.right},
        };
        
        public static SwipeDirection GetSwipeDirection()
        {
            SwipeDirection swipeDirection = SwipeDirection.None;
            foreach (LeanFinger leanFinger in LeanTouch.Fingers)
            {
                bool swiped = leanFinger.Swipe;
                if (!swiped)
                {
                    continue;
                }

                swipeDirection |= TestSwipeDirection(leanFinger, SwipeDirection.Left);
                swipeDirection |= TestSwipeDirection(leanFinger, SwipeDirection.Right);
            }

            return swipeDirection;
        }

        private static SwipeDirection TestSwipeDirection(LeanFinger finger, SwipeDirection direction)
        {
            return HasFingerSwipedInDirection(finger, _swipeDirectionVectors[direction]) ? direction : SwipeDirection.None;
        }
        
        private static bool HasFingerSwipedInDirection(LeanFinger finger, Vector2 direction)
        {
            Vector2 swipeDirection = finger.SwipeScaledDelta;
            float swipeValue = Vector2.Dot(swipeDirection.normalized, direction.normalized);
            CircumDebug.Log($"Testing swipe direction {direction} => swipe value of {swipeValue}");
            return swipeValue > SwipeDirectionThreshold;
        }
    }
}