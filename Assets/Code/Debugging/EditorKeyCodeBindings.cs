using UnityEngine;

namespace Code.Debugging
{
    /// <summary>
    /// Place to put all editor keyboard shortcuts in one place and wrapped as editor only
    /// </summary>
    public static class EditorKeyCodeBindings
    {
#if UNITY_EDITOR
        public static KeyCode NextLevel => KeyCode.RightArrow;
        public static KeyCode PreviousLevel => KeyCode.LeftArrow;
        public static KeyCode SwitchToKeyboardInput => KeyCode.K;
        public static KeyCode SwitchToControllerInput => KeyCode.C;
        public static KeyCode SwitchToMouseInput => KeyCode.M;
        public static KeyCode SwitchToUIInput => KeyCode.U;
        public static KeyCode PlayerInvulnerable => KeyCode.I;
        public static KeyCode ShowEscape => KeyCode.E;
#endif
    }
}