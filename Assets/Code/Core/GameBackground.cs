using System;
using Code.Level.Player;
using UnityEngine;

namespace Code.Core
{
    public class GameBackground : MonoBehaviour
    {
        [SerializeField] private GameBackgroundsManifest _gameBackgroundsManifest;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private string _textureParameterName = "Texture";

        private int _textureParameterId;
        
        public static Action GameBackgroundChanged { get; set; } = () => { };

        private void Start()
        {
            _textureParameterId = Shader.PropertyToID(_textureParameterName);
            
            GameBackgroundChanged += UpdateGameBackground;
            UpdateGameBackground();
        }

        private void OnDestroy()
        {
            GameBackgroundChanged -= UpdateGameBackground;
        }

        private void UpdateGameBackground()
        {
            CircumOptions playerOptions = PersistentDataManager.Instance.Options;
            int backgroundIndex = playerOptions.GameBackgroundIndex;
            Texture2D backgroundTexture = _gameBackgroundsManifest.BackgroundTextures[backgroundIndex];
            _renderer.material.SetTexture(_textureParameterId, backgroundTexture);
        }
    }
}