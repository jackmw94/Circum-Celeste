using System;
using System.Diagnostics;
using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.VFX
{
    [DefaultExecutionOrder(-1)]
    public class QualityLevelControlledObjects : MonoBehaviour
    {
        [Serializable]
        public class QualityLevelObject
        {
            [field: SerializeField] public int QualityLevelIndex { get; set; }
            [field: SerializeField] public string QualityLevelName_Readonly { get; set; }
            [field: SerializeField] public GameObject LevelObject { get; set; }
        }

        [SerializeField] private QualityLevelObject[] _qualityLevelObject;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (_qualityLevelObject == null) return;
            
            string[] qualityLevelsNames = QualitySettings.names;
            _qualityLevelObject.ApplyFunction(p =>
            {
                bool indexIsInRange = p.QualityLevelIndex >= 0 && p.QualityLevelIndex < qualityLevelsNames.Length;
                p.QualityLevelName_Readonly = indexIsInRange ? qualityLevelsNames[p.QualityLevelIndex] : "[out of range]";
            });
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        private void OnEnable()
        {
            SetActiveObjectForCurrentQualityLevel();
        }

        private void SetActiveObjectForCurrentQualityLevel()
        {
            int currentQualityLevel = QualitySettings.GetQualityLevel();
            bool foundObjectForLevel = false;

            foreach (QualityLevelObject qualityLevelObject in _qualityLevelObject)
            {
                bool activeForCurrentQualityLevel = qualityLevelObject.QualityLevelIndex == currentQualityLevel;
                qualityLevelObject.LevelObject.SetActiveSafe(activeForCurrentQualityLevel);

                foundObjectForLevel |= activeForCurrentQualityLevel;
            }

            if (!foundObjectForLevel)
            {
                string qualityLevelName = QualitySettings.names[currentQualityLevel];
                CircumDebug.LogError($"No object was defined for quality level {qualityLevelName}");
            }
        }

        public GameObject GetActiveObjectForCurrentQualityLevel()
        {
            int currentQualityLevel = QualitySettings.GetQualityLevel();
            
            foreach (QualityLevelObject qualityLevelObject in _qualityLevelObject)
            {
                if (qualityLevelObject.QualityLevelIndex == currentQualityLevel)
                {
                    return qualityLevelObject.LevelObject;
                }
            }

            string qualityLevelName = QualitySettings.names[currentQualityLevel];
            throw new NullReferenceException($"No object was defined for quality level {qualityLevelName}");
        }
    }
}