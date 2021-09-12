using UnityEngine.Rendering.Universal;

namespace Code.VFX
{
    public class AnimateHueShift : AnimatePostProcessingBase<ColorAdjustments>
    {
        protected override void SetValue(ColorAdjustments component, float value)
        {
            component.hueShift.value = value;
        }
    }
}