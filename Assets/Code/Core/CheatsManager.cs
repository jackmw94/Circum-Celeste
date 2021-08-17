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
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                EnemyHealthLossDisabled = !EnemyHealthLossDisabled;
            }
        }
    }
#endif
}