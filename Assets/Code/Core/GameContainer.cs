﻿using Code.Level;
using Code.Level.Player;
using Code.UI;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
    public class GameContainer : SingletonMonoBehaviour<GameContainer>
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private HealthUI _healthUI;
        [SerializeField] private TimerUI _timerUI;
        [SerializeField] private UIInputElementsContainer _uiInputElementsContainer;
        
        public LevelManager LevelManager => _levelManager;
        public HealthUI HealthUI => _healthUI;
        public TimerUI TimerUI => _timerUI;
        public UIInputElementsContainer UIInputElementsContainer => _uiInputElementsContainer;
    }
}