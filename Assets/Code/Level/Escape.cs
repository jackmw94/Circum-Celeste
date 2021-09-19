using Code.Core;
using Code.VFX;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class Escape : Collectable
    {
        private ISwitchable[] _switchVfxProperties = null;

        private ISwitchable[] SwitchVfxProperties
        {
            get { return _switchVfxProperties ??= GetComponentsInChildren<ISwitchable>(); }
        }
        
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
            SetSwitchVfxPropertiesOn();
            VfxManager.Instance.SpawnVfx(VfxType.PlayerEscaped, transform.position);
        }

        public void Reset()
        {
            SetSwitchVfxPropertiesOff();
        }

        [ContextMenu(nameof(SetSwitchVfxPropertiesOn))]
        private void SetSwitchVfxPropertiesOn() => SwitchVfxProperties.ApplyFunction(p => p.SetOnOff(true));
        
        [ContextMenu(nameof(SetSwitchVfxPropertiesOff))]
        private void SetSwitchVfxPropertiesOff() => SwitchVfxProperties.ApplyFunction(p => p.SetOnOff(false));
    }
}