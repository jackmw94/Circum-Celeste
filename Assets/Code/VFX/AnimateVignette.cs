using UnityEngine.Rendering.Universal;

namespace Code.VFX
{
    public class AnimateVignette : AnimatePostProcessingBase<Vignette>
    {
        protected override void SetValue(Vignette component, float value)
        {
            component.intensity.value = value;
        }
    }
}