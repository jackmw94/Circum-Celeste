using System.Collections.Generic;
using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.VFX
{
    public class VfxManager : SingletonMonoBehaviour<VfxManager>
    {
        private float MaximumVfxDuration = 5f;
        
        [SerializeField] private VfxManifest _manifest;
        
        private readonly Dictionary<VfxType, VfxData> _vfxByType = new Dictionary<VfxType, VfxData>();

        private void Awake()
        {
            RegenerateVfxDictionary();
        }
        
        public void SpawnVfx(VfxType vfxType) => SpawnVfx(vfxType, Vector3.zero, Vector3.zero);
        public void SpawnVfx(VfxType vfxType, Vector3 position) => SpawnVfx(vfxType, position, Vector3.zero);

        public void SpawnVfx(VfxType vfxType, Vector3 position, Vector3 direction)
        {
            if (!_vfxByType.TryGetValue(vfxType, out VfxData vfxData))
            {
                Debug.LogError($"Could not find vfx for type {vfxType}");
                return;
            }
            
            GameObject vfxInstance = Instantiate(vfxData.Prefab);
            Destroy(vfxInstance, vfxData.MaximumDuration);
            
            DirectionalVfx directionalVfx = vfxInstance.GetComponent<DirectionalVfx>();
            if (directionalVfx)
            {
                directionalVfx.Initialise(position, direction);
            }
        }

        [ContextMenu(nameof(RegenerateVfxDictionary))]
        private void RegenerateVfxDictionary()
        {
            _vfxByType.Clear();
            foreach (VfxData vfxValue in _manifest.VfxValues)
            {
                _vfxByType.Add(vfxValue.VfxType, vfxValue);
            }
        }
    }
}