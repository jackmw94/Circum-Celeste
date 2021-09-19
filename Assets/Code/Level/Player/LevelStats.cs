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
        
        public void UpdateFastestRecording(LevelRecording levelRecording, bool perfect, float goldTime, out BadgeData newBadgeData, out bool replacedExistingFastestTime, out bool replacedPerfectTime)
        {
            newBadgeData = new BadgeData();
            replacedPerfectTime = false;

            UpdateFastestRecordingInternal(levelRecording, ref FastestLevelRecording, goldTime, out _, out bool firstGold, out replacedExistingFastestTime);
            newBadgeData.HasGoldTime = firstGold;

            if (!perfect)
            {
                return;
            }
            
            UpdateFastestRecordingInternal(levelRecording, ref FastestPerfectLevelRecording, goldTime, out bool firstPerfect, out bool firstPerfectGold, out replacedPerfectTime);

            // this messing around with replaced time flags solves the awkward case where the user beats a previous imperfect time with a faster (first) perfect time
            // naively the perfect value would overwrite the regular value and since the perfect time was first it wouldn't overwrite despite being an replacement of the existing regular time
            replacedExistingFastestTime |= replacedPerfectTime;
            
            newBadgeData.IsPerfect = firstPerfect;
            newBadgeData.HasPerfectGoldTime = firstPerfectGold;
        }

        private static bool LevelRecordingExists(LevelRecording levelRecording)
        {
            return levelRecording != null && levelRecording.RecordingData.FrameData.Count > 0;
        }

        private void UpdateFastestRecordingInternal(LevelRecording levelRecording, ref LevelRecording currentRecording, float goldTime, out bool firstEntry, out bool firstGold, out bool replacedExistingFastestTime)
        {
            firstEntry = false;
            firstGold = false;
            replacedExistingFastestTime = false;
            
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
                firstGold = !hadBeatGold && hasNowBeatGold;
                replacedExistingFastestTime = true;
                _isDirty = true;
            }
        }
        
        public static bool TryLoadLevelStats(string levelName, out LevelStats levelStats)
        {
            string key = PlayerPrefsKeys.LevelStats(levelName);
            
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

            string key = PlayerPrefsKeys.LevelStats(levelName);
            
            CircumPlayerPrefs.SetString(key, compressed);

            levelStats._isDirty = false;
            
            CircumDebug.Log($"Saved level stats for {levelName} ({levelStats.FastestLevelRecording.RecordingData.FrameData.Count} fastest level frames)");
        }

        public static void ResetStats(string levelName)
        {
            string key = PlayerPrefsKeys.LevelStats(levelName);
            
            CircumPlayerPrefs.DeleteKey(key);
        }
    }
}