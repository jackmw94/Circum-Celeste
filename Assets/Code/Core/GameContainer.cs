using Code.Level;
using Code.Level.Player;
using Code.UI;
using Code.VFX;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
    public class GameContainer : SingletonMonoBehaviour<GameContainer>
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private PostProcessingFeedback _postProcessingFeedback;
        [SerializeField] private HealthUI _healthUI;
        [SerializeField] private UIInputElementsContainer _uiInputElementsContainer;
        
        public LevelManager LevelManager => _levelManager;
        public PostProcessingFeedback PostProcessingFeedback => _postProcessingFeedback;
        public HealthUI HealthUI => _healthUI;
        public UIInputElementsContainer UIInputElementsContainer => _uiInputElementsContainer;
    }
}