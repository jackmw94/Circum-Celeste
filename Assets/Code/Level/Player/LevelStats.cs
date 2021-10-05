using System;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class LevelStats
    {
#if UNITY_EDITOR
        private static bool ForceOldSaveMethod => false;
#endif
        
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
                currentRecording.IsDirty = true;
                _isDirty = true;
            }
            else if (currentRecording.RecordingData.LevelTime > levelRecording.RecordingData.LevelTime)
            {
                bool hadBeatGold = currentRecording.HasBeatenGoldTime(goldTime);
                bool hasNowBeatGold = levelRecording.HasBeatenGoldTime(goldTime);
                currentRecording = levelRecording;
                firstGold = !hadBeatGold && hasNowBeatGold;
                replacedExistingFastestTime = true;
                currentRecording.IsDirty = true;
                _isDirty = true;
            }
        }
        
        public static bool TryLoadLevelStats(string levelName, out LevelStats levelStats)
        {
            string metaDataKey = PersistentDataKeys.LevelMetaStats(levelName);
            bool foundKey = PersistentDataHelper.HasKey(metaDataKey);
            if (foundKey)
            {
                return LoadLevelStats(levelName, out levelStats);
            }
            
            string oldLevelKey = PersistentDataKeys.LevelStats_Old(levelName);
            bool foundOldKey = PersistentDataHelper.HasKey(oldLevelKey);
            if (foundOldKey)
            {
                return LoadOldStats(levelName, out levelStats);
            }
            
            levelStats = null;
            return false;
        }

        private static bool LoadLevelStats(string levelName, out LevelStats levelStats)
        {
            string perfectRecordingKey = PersistentDataKeys.PerfectLevelRecording(levelName);
            string imperfectRecordingKey = PersistentDataKeys.ImperfectLevelRecording(levelName);

            string serializedPerfectRecording = PersistentDataHelper.GetString(perfectRecordingKey);
            string serializedImperfectRecording = PersistentDataHelper.GetString(imperfectRecordingKey);

            string decompressedPerfectRecording = TryDecompressData(serializedPerfectRecording);
            string decompressedImperfectRecording = TryDecompressData(serializedImperfectRecording);

            LevelRecording perfectRecording = JsonUtility.FromJson<LevelRecording>(decompressedPerfectRecording);
            LevelRecording imperfectRecording = JsonUtility.FromJson<LevelRecording>(decompressedImperfectRecording);

            levelStats = new LevelStats
            {
                FastestLevelRecording = imperfectRecording,
                FastestPerfectLevelRecording = perfectRecording
            };
            
            CircumDebug.Log($"Loaded {levelName} with new method");

            return true;
        }

        private static bool LoadOldStats(string levelName, out LevelStats levelStats)
        {
            string oldLevelKey = PersistentDataKeys.LevelStats_Old(levelName);
            string serializedStats = PersistentDataHelper.GetString(oldLevelKey);
            string decompressedStats = TryDecompressData(serializedStats);
            
            levelStats = JsonUtility.FromJson<LevelStats>(decompressedStats);
            
            // given that we load on awake, this is probably too early to save
            // safer to wait until we try save next
            if (levelStats.HasFastestLevelRecording) levelStats.FastestLevelRecording.IsDirty = true;
            if (levelStats.HasFastestPerfectLevelRecording) levelStats.FastestPerfectLevelRecording.IsDirty = true;
            levelStats._isDirty = true;

            CircumDebug.Log($"Loaded {oldLevelKey} with old method");
            
            return true;
        }

        private static string TryDecompressData(string possiblyCompressedData)
        {
            string decompressedData = possiblyCompressedData;
            if (!possiblyCompressedData.SaveDataIsProbablyJson())
            {
                decompressedData = possiblyCompressedData.Decompress();
            }
            return decompressedData;
        }

        public static void SaveLevelStats(string levelName, LevelStats levelStats)
        {
#if UNITY_EDITOR
            if (ForceOldSaveMethod)
            {
                OldSaveLevelStats(levelName, levelStats);
                return;
            }
#endif

            if (!levelStats._isDirty)
            {
                return;
            }
            
            bool metaDataIsDirty = levelStats.FastestLevelRecording.IsDirty || (levelStats.FastestPerfectLevelRecording?.IsDirty ?? false);
            if (metaDataIsDirty)
            {
                string metaDataKey = PersistentDataKeys.LevelMetaStats(levelName);
                LevelMetaData levelMetaData = new LevelMetaData(levelStats);
                string serializedMetaData = JsonUtility.ToJson(levelMetaData);
                PersistentDataHelper.SetString(metaDataKey, serializedMetaData, true);
            }

            string perfectRecordingKey = PersistentDataKeys.PerfectLevelRecording(levelName);
            TrySaveLevelRecording(perfectRecordingKey, levelStats.FastestPerfectLevelRecording);

            string imperfectRecordingKey = PersistentDataKeys.ImperfectLevelRecording(levelName);
            TrySaveLevelRecording(imperfectRecordingKey, levelStats.FastestLevelRecording);
            
            CircumDebug.Log($"Saved level stats for {levelName} ({levelStats.FastestLevelRecording.RecordingData.FrameData.Count} fastest level frames)");
        }
        
        public static void OldSaveLevelStats(string levelName, LevelStats levelStats)
        {
            string serialized = JsonUtility.ToJson(levelStats);
            string compressed = serialized.Compress();

            string key = PersistentDataKeys.LevelStats_Old(levelName);
            
            PersistentDataHelper.SetString(key, compressed, true);

            CircumDebug.Log($"Saved level stats for using old method for {levelName} ({levelStats.FastestLevelRecording.RecordingData.FrameData.Count} fastest level frames)");
        }

        private static void TrySaveLevelRecording(string key, LevelRecording levelRecording)
        {
            if (levelRecording == null || !levelRecording.IsDirty)
            {
                return;
            }
            
            string serialized = JsonUtility.ToJson(levelRecording);
            string compressed = serialized.Compress();
            
            PersistentDataHelper.SetString(key, compressed, true);
        }

        public static void ResetStats(string levelName)
        {
            string oldKey = PersistentDataKeys.LevelStats_Old(levelName);
            string metaKey = PersistentDataKeys.LevelMetaStats(levelName);
            string perfectRecordingKey = PersistentDataKeys.PerfectLevelRecording(levelName);
            string imperfectRecordingKey = PersistentDataKeys.ImperfectLevelRecording(levelName);
            
            PersistentDataHelper.DeleteKey(oldKey);
            PersistentDataHelper.DeleteKey(metaKey);
            PersistentDataHelper.DeleteKey(perfectRecordingKey);
            PersistentDataHelper.DeleteKey(imperfectRecordingKey);
        }
    }
}