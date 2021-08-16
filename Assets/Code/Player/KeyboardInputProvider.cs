using UnityEngine;

namespace Code.Player
{
    public class KeyboardInputProvider : InputProvider
    {
        public override Vector2 GetMovementInput()
        {
            Vector2 movement = Vector2.zero;
        
            if (Input.GetKey(KeyCode.W)) movement += Vector2.up;
            if (Input.GetKey(KeyCode.S)) movement += Vector2.down;
            if (Input.GetKey(KeyCode.A)) movement += Vector2.left;
            if (Input.GetKey(KeyCode.D)) movement += Vector2.right;

            return movement;
        }

        public override bool GetSlingInput()
        {
            return Input.GetKey(KeyCode.Space);
        }
    }
}