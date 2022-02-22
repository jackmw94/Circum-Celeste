using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityExtras.Core;

namespace Code.Level
{
    public class ShrinkObjects : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Transform[] _transforms;

        private Vector3[] _initialScales = null;
        private Coroutine _shrinkCoroutine = null;

        private Vector3[] InitialScales
        {
            get
            {
                if (_initialScales != null)
                {
                    return _initialScales;
                }
                
                _initialScales = new Vector3[_transforms.Length];
                for (int i = 0; i < _transforms.Length; i++)
                {
                    _initialScales[i] = _transforms[i].localScale;
                }
                return _initialScales;
            }
        }
        
        [UsedImplicitly]
        public void RunShrink()
        {
            if (_shrinkCoroutine != null)
            {
                StopCoroutine(_shrinkCoroutine);
            }

            _shrinkCoroutine = StartCoroutine(ShrinkCoroutine());
        }

        private IEnumerator ShrinkCoroutine()
        {
            yield return Utilities.LerpOverTime(0f, 1f, _duration, f =>
            {
                for (int i = 0; i < _transforms.Length; i++)
                {
                    _transforms[i].localScale = Vector3.Lerp(InitialScales[i], Vector3.zero, f);
                }
            });
        }

        public void Reset()
        {
            if (_shrinkCoroutine != null)
            {
                StopCoroutine(_shrinkCoroutine);
            }
            
            for (int i = 0; i < _transforms.Length; i++)
            {
                _transforms[i].localScale = InitialScales[i];
            }
        }
    }
}