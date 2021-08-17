using Code.Core;
using UnityEngine;

namespace Code.Level
{
    public class EnemyHealth : EntityHealth
    {
        protected override bool DoesObjectDamageUs(GameObject gameObj)
        {
#if UNITY_EDITOR
            if (CheatsManager.Instance.EnemyHealthLossDisabled)
            {
                return false;
            }
#endif
            
            return gameObj.IsOrbiter();
        }
    }
}