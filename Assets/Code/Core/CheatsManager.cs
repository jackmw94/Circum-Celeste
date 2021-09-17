using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
    public class CheatsManager : SingletonMonoBehaviour<CheatsManager>
    {
#if UNITY_EDITOR
        public bool PlayerHealthLossDisabled { get; private set; }

        private void Update()
        {
            if (Input.GetKeyDown(EditorKeyCodeBindings.PlayerInvulnerable))
            {
                PlayerHealthLossDisabled = !PlayerHealthLossDisabled;
                CircumDebug.Log($"Setting player invulnerability to {BoolToOnOff(PlayerHealthLossDisabled)}");
            }
        }

        private static string BoolToOnOff(bool value)
        {
            return value ? "on" : "off";
        }
#endif
    }
}