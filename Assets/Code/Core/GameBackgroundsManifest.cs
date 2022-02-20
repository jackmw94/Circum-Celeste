using UnityEngine;

namespace Code.Core
{
    [CreateAssetMenu(menuName = "Create GameBackgroundsManifest", fileName = "GameBackgroundsManifest", order = 0)]
    public class GameBackgroundsManifest : ScriptableObject
    {
        [SerializeField] private Texture2D[] _backgroundTextures;

        public Texture2D[] BackgroundTextures => _backgroundTextures;
    }
}