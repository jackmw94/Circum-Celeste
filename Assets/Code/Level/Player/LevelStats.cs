using System;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class LevelStats
    {
        [field: SerializeField] public LevelRecording FastestLevelRecording;
        [field: SerializeField] public LevelRecording FastestPerfectLevelRecording;

        private bool _isDirty = false;
        
        private static string PlayerPrefsKey(string levelName) => $"Circum_PlayerStats_{levelName}";
        
        public void UpdateFastestRecording(LevelRecording levelRecording, bool perfect, out bool firstPerfect)
        {
            firstPerfect = false;
            UpdateFastestRecordingInternal(levelRecording, ref FastestLevelRecording, out _);
            
            if (perfect)
            {
                UpdateFastestRecordingInternal(levelRecording, ref FastestPerfectLevelRecording, out firstPerfect);
            }
        }

        private void UpdateFastestRecordingInternal(LevelRecording levelRecording, ref LevelRecording currentRecording, out bool firstEntry)
        {
            firstEntry = false;
            
            if (currentRecording == null)
            {
                currentRecording = levelRecording;
                firstEntry = true;
                _isDirty = true;
            }
            else if (currentRecording.RecordingData.LevelTime > levelRecording.RecordingData.LevelTime)
            {
                currentRecording = levelRecording;
                _isDirty = true;
            }
        }
        
        public static bool TryLoadLevelStats(string levelName, out LevelStats levelStats)
        {
            string key = PlayerPrefsKey(levelName);
            
            if (CircumPlayerPrefs.HasKey(key))
            {
                string serializedStats = CircumPlayerPrefs.GetString(key);
                string decompressedStats = serializedStats;
                if (!serializedStats.SaveDataIsProbablyJson())
                {
                    decompressedStats = serializedStats.Decompress();
                }
                levelStats = JsonUtility.FromJson<LevelStats>(decompressedStats);
                return true;
            }

            levelStats = null;
            return false;
        }

        public static void SaveLevelStats(string levelName, LevelStats levelStats)
        {
            if (!levelStats._isDirty)
            {
                return;
            }
            
            string serialized = JsonUtility.ToJson(levelStats);
            string compressed = serialized.Compress();

            string key = PlayerPrefsKey(levelName);
            
            CircumPlayerPrefs.SetString(key, compressed);
            CircumPlayerPrefs.Save();

            levelStats._isDirty = false;
            
            CircumDebug.Log($"Saved level stats for {levelName} ({levelStats.FastestLevelRecording.RecordingData.FrameData.Count} fastest level frames)");
        }
    }
}