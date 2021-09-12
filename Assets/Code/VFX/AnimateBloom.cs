using UnityEngine.Rendering.Universal;

namespace Code.VFX
{
    public class AnimateBloom : AnimatePostProcessingBase<Bloom>
    {
        protected override void SetValue(Bloom component, float value)
        {
            component.intensity.value = value;
        }
    }
}