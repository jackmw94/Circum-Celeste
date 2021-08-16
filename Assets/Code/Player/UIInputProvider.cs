using System;
using Code.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Player
{
    public class UIInputProvider : InputProvider
    {
        private UIInputElementsContainer _uiInputElements;

        protected override void Initialise()
        {
            base.Initialise();
            _uiInputElements = UIInputElementsContainer.Instance;
        }
        
        public override Vector2 GetMovementInput()
        {
            return _uiInputElements.MoverHandle.Movement;
        }

        public override bool GetSlingInput()
        {
            return _uiInputElements.SlingButton.IsHeld;
        }
    }
}