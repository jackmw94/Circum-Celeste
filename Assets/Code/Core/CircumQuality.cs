using System;
using System.Collections.Generic;
using Code.Debugging;
using UnityEngine;

namespace Code.Core
{
    public static class CircumQuality
    {
        private const int HighQualityEstimateSystemMemoryThreshold = 1024;
        
        public enum CircumQualitySetting
        {
            Medium, High
        }
        
        private static readonly Dictionary<CircumQualitySetting, int> CircumQualitySettingToQualityIndex = new Dictionary<CircumQualitySetting, int>()
        {
            {CircumQualitySetting.Medium, 1},
            {CircumQualitySetting.High, 2}
        };

        public static void SetCircumQualityLevel(CircumQualitySetting circumQualitySetting)
        {
            if (!CircumQualitySettingToQualityIndex.TryGetValue(circumQualitySetting, out int qualityIndex))
            {
                throw new ArgumentException($"There was no quality index for setting {circumQualitySetting}");
            }
            
            QualitySettings.SetQualityLevel(qualityIndex);
        }
        
        public static CircumQualitySetting EstimateBestDefaultQualitySetting()
        {
            int systemMemorySize = SystemInfo.systemMemorySize;
            bool useHighQualitySettings = systemMemorySize >= HighQualityEstimateSystemMemoryThreshold || SystemInfo.supportsComputeShaders;
            CircumDebug.Log($"System memory size is at {systemMemorySize} or has computer shaders {SystemInfo.supportsComputeShaders} so using {(useHighQualitySettings ? "high" : "medium")} quality settings");
            return useHighQualitySettings ? CircumQualitySetting.High : CircumQualitySetting.Medium;
        }
    }
}