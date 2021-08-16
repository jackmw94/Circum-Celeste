﻿using UnityEngine;

namespace Code.Player
{
    public class MouseInputProvider : InputProvider
    {
        protected override bool CursorLocked => true;
        
        public override Vector2 GetMovementInput()
        {
            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");
            return new Vector2(xAxis, yAxis);
        }

        public override bool GetSlingInput()
        {
            return Input.GetMouseButton(0);
        }
    }
}