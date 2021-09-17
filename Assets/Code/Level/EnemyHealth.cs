using Code.Core;
using UnityEngine;

namespace Code.Level
{
    public class EnemyHealth : EntityHealth
    {
        protected override bool DoesObjectDamageUs(GameObject gameObj)
        {
            return gameObj.IsOrbiter();
        }
    }
}