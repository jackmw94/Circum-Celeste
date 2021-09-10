using UnityEngine.VFX;

namespace Code.VFX
{
    public class SwitchVfxInt : SwitchVfxPropertyBase<int>
    {
        protected override void ApplyValue(VisualEffect vfx, int propertyId, int value)
        {
            vfx.SetInt(propertyId, value);
        }
    }
}