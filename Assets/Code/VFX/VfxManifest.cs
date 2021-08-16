using UnityEngine;

namespace Code.VFX
{
    [CreateAssetMenu(menuName = "Create VfxManifest", fileName = "VfxManifest", order = 0)]
    public class VfxManifest : ScriptableObject
    {
        [SerializeField] private VfxData[] _vfxValues;

        public VfxData[] VfxValues => _vfxValues;
    }
}