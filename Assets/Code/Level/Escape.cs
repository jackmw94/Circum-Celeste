using Code.Core;
using Code.VFX;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class Escape : Collectable
    {
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
            VfxManager.Instance.SpawnVfx(VfxType.PlayerEscaped, transform.position);
        }
    }
}