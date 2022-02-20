using UnityEngine;

namespace Code.Level
{
    public class MeshLevelOverlay : LevelOverlay
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Collider _collider;
        
        protected override Material GetMaterial()
        {
            return _renderer.material;
        }

        protected override void SetColliderOnOff(bool on)
        {
            _collider.enabled = on;
        }
    }
}