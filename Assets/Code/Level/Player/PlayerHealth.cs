using Code.Core;
using Code.Juice;
using UnityEngine;

namespace Code.Level.Player
{
    public class PlayerHealth : EntityHealth
    {
        public bool OrbiterCanDamage { get; set; } = true;
        
        protected override void OnHitTaken()
        {
            base.OnHitTaken();

            GameContainer gameContainer = GameContainer.Instance;
            
            HealthUI healthUI = gameContainer.HealthUI;
            healthUI.UpdateHealthBar(0, HealthFraction);
            
            Feedbacks.Instance.TriggerFeedback(Feedbacks.FeedbackType.PlayerDamaged);
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
            
            return gameObj.IsEnemy() || (OrbiterCanDamage && gameObj.IsOrbiter());
        }

        public void TutorialDisableOnStayDamage()
        {
            _onStayDamageDelay = float.MaxValue;
        }
    }
}