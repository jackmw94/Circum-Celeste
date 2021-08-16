using UnityEngine;

namespace Code.Player
{
    public class ControllerInputProvider : InputProvider
    {
        public override Vector2 GetMovementInput()
        {
            float xAxis = Input.GetAxis("Horizontal");
            float yAxis = Input.GetAxis("Vertical");
            return new Vector2(xAxis, yAxis);
        }

        public override bool GetSlingInput()
        {
            return Input.GetButton("Jump");
        }
    }
}