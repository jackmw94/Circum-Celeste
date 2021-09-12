using UnityEngine.Rendering.Universal;

namespace Code.VFX
{
    public class AnimateChromaticAberration : AnimatePostProcessingBase<ChromaticAberration>
    {
        protected override void SetValue(ChromaticAberration component, float value)
        {
            component.intensity.value = value;
        }
    }
}