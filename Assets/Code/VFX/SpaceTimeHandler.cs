using System;
using System.Collections.Generic;
using System.Diagnostics;
using Code.Behaviours;
using UnityEngine;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.Singletons;

namespace Code.VFX
{
    public class SpaceTimeHandler : SingletonMonoBehaviour<SpaceTimeHandler>
    {
        [SerializeField] private MaterialProvider _materialProvider;
        [SerializeField] private float _extent = 10f;

        private readonly HashSet<GameObject> _spaceTimeObjects = new HashSet<GameObject>();
        private static Vector3 BeyondBoundsPosition = -Vector3.one * 100f;
        private static readonly Dictionary<int, int> _counterToShaderProperty = new Dictionary<int, int>()
        {
            {1, Shader.PropertyToID("Position1")},
            {2, Shader.PropertyToID("Position2")},
            {3, Shader.PropertyToID("Position3")},
            {4, Shader.PropertyToID("Position4")},
            {5, Shader.PropertyToID("Position5")},
            {6, Shader.PropertyToID("Position6")},
            {7, Shader.PropertyToID("Position7")},
            {8, Shader.PropertyToID("Position8")},
        };

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_materialProvider)
            {
                _materialProvider = GetComponent<MaterialProvider>();
            }
        }

        private void Awake()
        {
            ClearAll();
        }

        private void OnDestroy()
        {
            ClearAll();
        }

        private void ClearAll()
        {
            for (int i = 1; i <= 8; i++)
            {
                Clear(i);
            }
        }

        private void Clear(int positionNumber)
        {
            HandleObject(BeyondBoundsPosition, positionNumber, false);
        }

        public void RegisterObject(GameObject obj)
        {
            _spaceTimeObjects.Add(obj);
        }

        public void UnregisterObject(GameObject obj)
        {
            int removePositionNumber = _spaceTimeObjects.Count;
            _spaceTimeObjects.Remove(obj);
            Clear(removePositionNumber);
        }
        
        private void Update()
        {
            int counter = 1;
            foreach (GameObject spaceTimeObject in _spaceTimeObjects)
            {
                HandleObject(spaceTimeObject.transform.position, counter);
                counter++;
            }
        }

        private void HandleObject(Vector3 worldPosition, int count, bool clamp = true)
        {
            float normalisedX = InverseLerp(-_extent,  _extent, worldPosition.x, clamp);
            float normalisedY = InverseLerp(-_extent, _extent, worldPosition.y, clamp);
            Material material = _materialProvider.GetMaterial();
            material.SetVector(_counterToShaderProperty[count], new Vector4(normalisedX, normalisedY));
        }

        private float InverseLerp(float a, float b, float value, bool clamp)
        {
            return clamp ? Mathf.InverseLerp(a, b, value) : Utilities.UnclampedInverseLerp(a, b, value);
        }
    }
}