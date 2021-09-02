using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
    public class CheatsManager : SingletonMonoBehaviour<CheatsManager>
    {
#if UNITY_EDITOR
        public bool PlayerHealthLossDisabled { get; private set; }
        public bool EnemyHealthLossDisabled { get; private set; }

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
#endif
    }
}