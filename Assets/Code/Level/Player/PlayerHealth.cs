﻿using Code.Core;
using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerHealth : EntityHealth
    {
        protected override bool DoesObjectDamageUs(GameObject gameObj)
        {
#if UNITY_EDITOR
            if (CheatsManager.Instance.PlayerHealthLossDisabled)
            {
                return false;
            }
#endif
            
            return gameObj.IsOrbiter() || gameObj.IsEnemy();
        }
    }
}