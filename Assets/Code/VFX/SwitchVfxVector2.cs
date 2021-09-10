using UnityEngine;
using UnityEngine.VFX;

namespace Code.VFX
{
    public class SwitchVfxVector2 : SwitchVfxPropertyBase<Vector2>
    {
        protected override void ApplyValue(VisualEffect vfx, int propertyId, Vector2 value)
        {
            vfx.SetVector2(propertyId, value);
        }
    }
}