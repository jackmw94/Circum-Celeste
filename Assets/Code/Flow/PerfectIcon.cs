﻿using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class PerfectIcon : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private string _shaderPropertyName = "ShimmerPosition";
        [SerializeField] private float _shaderOffValue = -0.5f;
        [SerializeField] private float _shaderOnValue = 1.5f;
        [SerializeField] private float _animateInDelay = 1.5f;
        
        [SerializeField, Tooltip("Duration from 0->1, if shader off and on values are -1 and 2 respectively then total animation will take 3 times as long as this value")] 
        private float _animationUnitDuration = 0.5f;

        private Coroutine _showHideCoroutine = null;
        private int _shaderPropertyId;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }
        }

        private void Awake()
        {
            _shaderPropertyId = Shader.PropertyToID(_shaderPropertyName);
        }

        public void ShowHidePerfectIcon(bool show, bool instant)
        {
            if (_showHideCoroutine != null)
            {
                StopCoroutine(_showHideCoroutine);
            }

            _showHideCoroutine = StartCoroutine(ShowHideCoroutine(show, instant));
        }

        private IEnumerator ShowHideCoroutine(bool show, bool instant)
        {
            float initialValue = _image.material.GetFloat(_shaderPropertyId);
            float targetValue = show ? _shaderOnValue : _shaderOffValue;
            float zeroToOneDuration = instant || !show ? 0f : _animationUnitDuration;

            if (!instant)
            {
                yield return new WaitForSeconds(_animateInDelay);
            }
            
            yield return Utilities.LerpOverTime(initialValue, targetValue, zeroToOneDuration, f =>
            {
                _image.material.SetFloat(_shaderPropertyId, f);
            });
        }
    }
}