using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
#if UNITY_EDITOR
    public class CheatsManager : SingletonMonoBehaviour<CheatsManager>
    {
        public bool PlayerHealthLossDisabled;
        public bool EnemyHealthLossDisabled;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                PlayerHealthLossDisabled = !PlayerHealthLossDisabled;
                CircumDebug.Log($"Setting player invulnerability to {BoolToOnOff(PlayerHealthLossDisabled)}");
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                EnemyHealthLossDisabled = !EnemyHealthLossDisabled;
                CircumDebug.Log($"Setting enemy invulnerability to {BoolToOnOff(EnemyHealthLossDisabled)}");
            }
        }

        private static string BoolToOnOff(bool value)
        {
            return value ? "on" : "off";
        }
    }
#endif
}