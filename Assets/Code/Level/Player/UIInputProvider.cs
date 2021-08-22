﻿using Code.Core;
using Code.UI;
using UnityEngine;

namespace Code.Level.Player
{
    public class UIInputProvider : InputProvider
    {
        private UIInputElementsContainer _uiInputElements;

        protected override void Initialise()
        {
            base.Initialise();
            _uiInputElements = GameContainer.Instance.UIInputElementsContainer;
        }
        
        public override Vector2 GetMovementInput()
        {
            return _uiInputElements.MoverHandle.Movement;
        }

        public override bool GetSlingInput()
        {
            bool isHeld = _uiInputElements.SlingButton.IsHeld;
            
#if UNITY_EDITOR
            isHeld |= Input.GetKey(KeyCode.Space);
#endif
            
            return isHeld;
        }
    }
}