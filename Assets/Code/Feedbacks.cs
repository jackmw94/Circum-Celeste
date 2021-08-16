using MoreMountains.Feedbacks;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code
{
    public class Feedbacks : SingletonMonoBehaviour<Feedbacks>
    {
        [SerializeField] private MMFeedback _orbiterHitsPlayer;

        public void SetTriggerOrbiterHitsPlayerFeedbackActive(bool active)
        {
            if (!active)
            {
                _orbiterHitsPlayer.Stop(Vector3.zero);
            }
            else
            {
                _orbiterHitsPlayer.Play(Vector3.zero);
            }
        }
    }
}