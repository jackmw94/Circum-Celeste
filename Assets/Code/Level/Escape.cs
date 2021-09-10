using Code.Core;
using Code.VFX;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class Escape : Collectable
    {
        private ISwitchable[] _switchVfxProperties;
        
        private void Awake()
        { 
            _switchVfxProperties = GetComponentsInChildren<ISwitchable>();
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

        [ContextMenu(nameof(SetSwitchVfxPropertiesOn))]
        private void SetSwitchVfxPropertiesOn() => _switchVfxProperties.ApplyFunction(p => p.SetOnOff(true));
        
        [ContextMenu(nameof(SetSwitchVfxPropertiesOff))]
        private void SetSwitchVfxPropertiesOff() => _switchVfxProperties.ApplyFunction(p => p.SetOnOff(false));
    }
}