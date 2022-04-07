using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.VFX
{
    public class AppFeedbacks : SingletonMonoBehaviour<AppFeedbacks>
    {
        [SerializeField] private Animator _cometsAnimator;
        [SerializeField] private ParticleSystem _menuEdgeParticles;
        [SerializeField] private AnimateHueShift _animateHueShift;
        
        private static readonly int PlayParameterId = Animator.StringToHash("Play");

        [ContextMenu(nameof(TriggerComets))]
        public void TriggerComets()
        {
            _cometsAnimator.SetTrigger(PlayParameterId);
        }

        [ContextMenu(nameof(TriggerMenuEdgeBurst))]
        public void TriggerMenuEdgeBurst()
        {
            _menuEdgeParticles.Play();
        }

        [ContextMenu(nameof(TriggerHueShift))]
        public void TriggerHueShift()
        {
            _animateHueShift.TriggerAnimation();
        }
    }
}