using UnityEngine;

namespace Code.VFX
{
    public class InitialiseAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [Space(15)]
        [SerializeField] private string _animatorPropertyName;
        [SerializeField] private float _valueMin;
        [SerializeField] private float _valueMax;

        private void Awake()
        {
            float value = Random.Range(_valueMin, _valueMax);
            _animator.SetFloat(_animatorPropertyName, value);
        }
    }
}