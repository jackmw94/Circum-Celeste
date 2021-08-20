﻿using Code.Core;
using Code.VFX;
using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerHealth : EntityHealth
    {
        protected override void OnHitTaken()
        {
            base.OnHitTaken();

            GameContainer gameContainer = GameContainer.Instance;
            
            HealthUI healthUI = gameContainer.HealthUI;
            healthUI.UpdateHealthBar(0, HealthFraction);

            PostProcessingFeedback postProcessingFeedback = gameContainer.PostProcessingFeedback;
            postProcessingFeedback.TriggerDamageFeedback();
        }

        protected override void OnHealthReset()
        {
            base.OnHealthReset();
            
            GameContainer gameContainer = GameContainer.Instance;
            gameContainer.HealthUI.UpdateHealthBar(0, HealthFraction);
        }

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