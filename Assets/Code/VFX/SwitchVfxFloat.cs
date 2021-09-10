using UnityEngine.VFX;

namespace Code.VFX
{
    public class SwitchVfxFloat : SwitchVfxPropertyBase<float>
    {
        protected override void ApplyValue(VisualEffect vfx, int propertyId, float value)
        {
            vfx.SetFloat(propertyId, value);
        }
    }
}