﻿using System;
using UnityEngine;

namespace Code.Behaviours
{
    public class PulseColour : MonoBehaviour
    {
        [SerializeField] private Colourable _colourable;
        [Space(15)] 
        [SerializeField] private float _frequency;
        [SerializeField, ColorUsage(true, true)] private Color _pulsedColour;

        private Color _defaultColor;
        private float _startPulsingTime = 0f;

        private void Awake()
        {
            _defaultColor = _colourable.GetColour();
        }

        private void OnEnable()
        {
            _startPulsingTime = Time.time;
        }

        private void OnDisable()
        {
            _colourable.SetColour(_defaultColor);
        }

        private void Update()
        {
            float pulseTime = Time.time - _startPulsingTime;
            float sinVal = Mathf.Sin(pulseTime * _frequency - Mathf.PI / 2f);
            float lerpVal = sinVal / 2f + 0.5f;
            Color colour = Color.Lerp(_defaultColor, _pulsedColour, lerpVal);
            _colourable.SetColour(colour);
        }
    }
}