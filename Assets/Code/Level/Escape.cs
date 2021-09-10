using Code.Core;
using Code.VFX;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class Escape : Collectable
    {
        [SerializeField] private SwitchVfxProperty _switchVfxProperty;
        
        public override void LevelSetup()
        {
            base.LevelSetup();
            gameObject.SetActiveSafe(false);
        }

        protected override bool CanObjectCollect(GameObject other)
        {
            return other.IsPlayer();
        }
        
        protected override void CollectableCollected(Vector3 _)
        {
            _switchVfxProperty.SetOnOff(true);
            VfxManager.Instance.SpawnVfx(VfxType.PlayerEscaped, transform.position);
        }
    }
}