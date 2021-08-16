using System;
using UnityEngine;

namespace Code.VFX
{
    [Serializable]
    public class VfxData
    {
        [SerializeField] private VfxType _vfxType;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private float _maximumDuration = 5f;

        public VfxType VfxType => _vfxType;
        public GameObject Prefab => _prefab;
        public float MaximumDuration => _maximumDuration;
    }
}