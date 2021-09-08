using Code.Level;
using Code.Level.Player;
using Code.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
    public class GameContainer : SingletonMonoBehaviour<GameContainer>
    {
        [SerializeField] private HealthUI _healthUI;
        [FormerlySerializedAs("_timerUI")] [SerializeField] private CountdownTimerUI _countdownTimerUI;
        [SerializeField] private UIInputElementsContainer _uiInputElementsContainer;
        [SerializeField] private LevelTimeUI _levelTimeUI;
        
        public HealthUI HealthUI => _healthUI;
        public CountdownTimerUI CountdownTimerUI => _countdownTimerUI;
        public UIInputElementsContainer UIInputElementsContainer => _uiInputElementsContainer;
        public LevelTimeUI LevelTimeUI => _levelTimeUI;
    }
}