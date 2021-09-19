using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class ExampleMovingPlayer : MonoBehaviour
    {
        [SerializeField] private Escape _escape;
        [SerializeField] private Animator _animator;
        [SerializeField] private ShrinkObjects _shrinkObjects;

        public void EnableExampleMovingPlayer(bool examplePlayerEnabled)
        {
            gameObject.SetActiveSafe(examplePlayerEnabled);
            _shrinkObjects.Reset();
            _escape.Reset();

            if (examplePlayerEnabled)
            {
                _animator.Rebind();
                _animator.Update(0f);
            }
        }
    }
}