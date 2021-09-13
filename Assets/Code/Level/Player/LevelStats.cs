using System;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class LevelStats
    {
        public LevelRecording FastestLevelRecording;
        public LevelRecording FastestPerfectLevelRecording;

        private bool _isDirty = false;
        
        public bool HasFastestLevelRecording => LevelRecordingExists(FastestLevelRecording);
        public bool HasFastestPerfectLevelRecording => LevelRecordingExists(FastestPerfectLevelRecording);
        private static string PlayerPrefsKey(string levelName) => $"Circum_PlayerStats_{levelName}";
        
        public void UpdateFastestRecording(LevelRecording levelRecording, bool perfect, float goldTime, out BadgeData newBadgeData)
        {
            newBadgeData = new BadgeData();

            UpdateFastestRecordingInternal(levelRecording, ref FastestLevelRecording, goldTime, out _, out bool firstGold);
            newBadgeData.HasGoldTime = firstGold;

            if (!perfect)
            {
                return;
            }
            
            UpdateFastestRecordingInternal(levelRecording, ref FastestPerfectLevelRecording, goldTime, out bool firstPerfect, out bool firstPerfectGold);
            newBadgeData.IsPerfect = firstPerfect;
            newBadgeData.HasPerfectGoldTime = firstPerfectGold;
        }

        private static bool LevelRecordingExists(LevelRecording levelRecording)
        {
            return levelRecording != null && levelRecording.RecordingData.FrameData.Count > 0;
        }

        private void UpdateFastestRecordingInternal(LevelRecording levelRecording, ref LevelRecording currentRecording, float goldTime, out bool firstEntry, out bool firstGold)
        {
            firstEntry = false;
            firstGold = false;
            
            if (!LevelRecordingExists(currentRecording))
            {
                currentRecording = levelRecording;
                firstEntry = true;
                firstGold = levelRecording.RecordingData.LevelTime <= goldTime;
                _isDirty = true;
            }
            else if (currentRecording.RecordingData.LevelTime > levelRecording.RecordingData.LevelTime)
            {
                bool hadBeatGold = currentRecording.HasBeatenGoldTime(goldTime);
                bool hasNowBeatGold = levelRecording.HasBeatenGoldTime(goldTime);
                currentRecording = levelRecording;
                _isDirty = true;
                firstGold = !hadBeatGold && hasNowBeatGold;
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

            levelStats._isDirty = false;
            
            CircumDebug.Log($"Saved level stats for {levelName} ({levelStats.FastestLevelRecording.RecordingData.FrameData.Count} fastest level frames)");
        }

        public static void ResetStats(string levelName)
        {
            string key = PlayerPrefsKey(levelName);
            
            CircumPlayerPrefs.DeleteKey(key);
        }
    }
}