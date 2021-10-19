using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.VFX
{
    public class AppFeedbacks : SingletonMonoBehaviour<AppFeedbacks>
    {
        [SerializeField] private Animator _cometsAnimator;
        
        private static readonly int PlayParameterId = Animator.StringToHash("Play");

        public void TriggerComets()
        {
            _cometsAnimator.SetTrigger(PlayParameterId);
        }
    }
}