using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.VFX
{
    public class AppFeedbacks : SingletonMonoBehaviour<AppFeedbacks>
    {
        [SerializeField] private Animator _cometsAnimator;
        [SerializeField] private ParticleSystem _menuEdgeParticles;
        
        private static readonly int PlayParameterId = Animator.StringToHash("Play");

        public void TriggerComets()
        {
            _cometsAnimator.SetTrigger(PlayParameterId);
        }

        public void TriggerMenuEdgeBurst()
        {
            _menuEdgeParticles.Play();
        }
    }
}