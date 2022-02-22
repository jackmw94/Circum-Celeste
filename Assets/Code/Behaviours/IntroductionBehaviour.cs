using UnityEngine;

namespace Code.Behaviours
{
    public abstract class IntroductionBehaviour : MonoBehaviour
    {
        // empty abstract class to reference behaviours used to introduce game elements
        // was previously using Behaviour where I was referencing these
        // that lead to me dragging and dropping the wrong components! never again.

        public virtual void SetEnabled(bool isEnabled)
        {
            enabled = isEnabled;
        }
    }
}