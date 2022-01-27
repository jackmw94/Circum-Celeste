using System;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class LevelStats
    {
        [Obsolete] public LevelRecording FastestLevelRecording;
        [Obsolete] public LevelRecording FastestPerfectLevelRecording;

        public LevelRecording LevelRecording;

        // need to track this separately to the best recording since a gold time recording gets overwritten by a perfect one
        public bool HasPreviouslyCompletedInGoldTime = false;

        private bool _isDirty = false;

        public bool HasRecording => LevelRecordingExists(LevelRecording);

        
        public bool UpdateFastestRecording(LevelRecording newLevelRecording, float goldTime, out BadgeData newBadgeData, out bool replacedExistingRecording)
        {
            newBadgeData = new BadgeData();

            bool HasPerfectAndGoldTime(LevelRecording levelRecording)
            {
                return LevelRecordingExists(levelRecording) && levelRecording.IsPerfect && levelRecording.HasBeatenGoldTime(goldTime);
            }

            bool previouslyHadPerfectGoldTime = HasPerfectAndGoldTime(LevelRecording);

            UpdateFastestRecordingInternal(newLevelRecording, ref LevelRecording, goldTime, out bool firstPerfect, out bool firstGold, out replacedExistingRecording);
            newBadgeData.HasGoldTime = firstGold;
            newBadgeData.IsPerfect = firstPerfect;
            newBadgeData.HasPerfectGoldTime = HasPerfectAndGoldTime(newLevelRecording) && !previouslyHadPerfectGoldTime;

            return replacedExistingRecording;
        }

        private static bool LevelRecordingExists(LevelRecording levelRecording)
        {
            return levelRecording != null && levelRecording.RecordingData.FrameData.Count > 0;
        }

        private void UpdateFastestRecordingInternal(LevelRecording levelRecording, ref LevelRecording currentRecording, float goldTime, out bool firstPerfect, out bool firstGold, out bool replacedExistingLevel)
        {
            firstGold = false;
            firstPerfect = false;
            replacedExistingLevel = false;

            if (!LevelRecordingExists(currentRecording))
            {
                currentRecording = levelRecording;
                HasPreviouslyCompletedInGoldTime = levelRecording.LevelTime <= goldTime;
                firstGold = HasPreviouslyCompletedInGoldTime;
                firstPerfect = levelRecording.IsPerfect;
                replacedExistingLevel = true;
                
                currentRecording.IsDirty = true;
                _isDirty = true;
                return;
            }
            
            bool hadBeatGold = HasPreviouslyCompletedInGoldTime;
            bool hasNowBeatGold = levelRecording.HasBeatenGoldTime(goldTime);

            firstGold = !hadBeatGold && hasNowBeatGold;
            if (firstGold)
            {
                _isDirty = true;
                HasPreviouslyCompletedInGoldTime = true;
            }

            if (currentRecording.IsPerfect && !levelRecording.IsPerfect)
            {
                return;
            }

            bool beatenOnPerfectState = !currentRecording.IsPerfect && levelRecording.IsPerfect;
            bool beatenOnTime = currentRecording.LevelTime > levelRecording.LevelTime;
            if (!beatenOnPerfectState && !beatenOnTime)
            {
                return;
            }
            
            // new recording is better
            currentRecording = levelRecording;
            firstPerfect = beatenOnPerfectState;
            
            replacedExistingLevel = true;
            currentRecording.IsDirty = true;
            _isDirty = true;
        }
        
        public static bool TryLoadLevelStats(string levelName, float goldTime, out LevelStats levelStats)
        {
            string metaDataKey = PersistentDataKeys.LevelMetaStats(levelName);
            bool foundKey = PersistentDataHelper.HasKey(metaDataKey);
            
#if UNITY_EDITOR
            if (PersistentDataManager.Instance.ForceOldSaveMethod)
            {
                foundKey = false;
            }
#endif
            
            if (foundKey)
            {
                return LoadLevelStats(levelName, out levelStats);
            }
            
            string oldLevelKey = PersistentDataKeys.LevelStats_Old(levelName);
            bool foundOldKey = PersistentDataHelper.HasKey(oldLevelKey);
            if (foundOldKey)
            {
                return LoadOldStats(levelName, goldTime, out levelStats);
            }
            
            levelStats = null;
            return false;
        }

        private static bool LoadLevelStats(string levelName, out LevelStats levelStats)
        {
            string recordingKey = PersistentDataKeys.LevelRecording(levelName);

            string serializedRecording = PersistentDataHelper.GetString(recordingKey);
            string decompressedRecording = TryDecompressData(serializedRecording);

            LevelRecording levelRecording = JsonUtility.FromJson<LevelRecording>(decompressedRecording);

            levelStats = new LevelStats
            {
                LevelRecording = levelRecording
            };
            
            CircumDebug.Log($"Loaded {levelName} with new method");

            return true;
        }

        private static bool LoadOldStats(string levelName, float goldTime, out LevelStats levelStats)
        {
            // disabling to prevent compiler warning RE obsolete data. This function transfers the obsolete data to new
#pragma warning disable 612
            string oldLevelKey = PersistentDataKeys.LevelStats_Old(levelName);
            string serializedStats = PersistentDataHelper.GetString(oldLevelKey);
            string decompressedStats = TryDecompressData(serializedStats);
            
            levelStats = JsonUtility.FromJson<LevelStats>(decompressedStats);

            levelStats.SetRecordingFromOldData(levelStats.FastestLevelRecording, levelStats.FastestPerfectLevelRecording, goldTime);

            levelStats._isDirty = true;
            levelStats.HasPreviouslyCompletedInGoldTime =
                levelStats.FastestLevelRecording.HasBeatenGoldTime(goldTime) || 
                levelStats.FastestPerfectLevelRecording.HasBeatenGoldTime(goldTime);
            
            if (levelStats.HasRecording) levelStats.LevelRecording.IsDirty = true;
#pragma warning restore 612
            
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
            if (PersistentDataManager.Instance.ForceOldSaveMethod)
            {
                OldSaveLevelStats(levelName, levelStats);
                return;
            }
#endif

            if (!levelStats._isDirty)
            {
                return;
            }

            string metaDataKey = PersistentDataKeys.LevelMetaStats(levelName);
            LevelMetaData levelMetaData = new LevelMetaData(levelStats);
            string serializedMetaData = JsonUtility.ToJson(levelMetaData);
            PersistentDataHelper.SetString(metaDataKey, serializedMetaData, true);

            string recordingKey = PersistentDataKeys.LevelRecording(levelName);
            TrySaveLevelRecording(recordingKey, levelStats.LevelRecording);

            CircumDebug.Log($"Saved level stats for {levelName} ({levelStats.LevelRecording.RecordingData.FrameData.Count} fastest level frames)");
        }
        
        public static void OldSaveLevelStats(string levelName, LevelStats levelStats)
        {
            // disabling to prevent compiler warning RE obsolete data. This function forces saving using the obsolete data for testing purposes
#pragma warning disable 612
            if (levelStats.LevelRecording != null && levelStats.LevelRecording.RecordingData.IsPerfect)
            {
                levelStats.FastestPerfectLevelRecording = levelStats.LevelRecording;
            }
            else
            {
                levelStats.FastestLevelRecording = levelStats.LevelRecording;
            }

            LevelRecording notSavedLevelRecording = levelStats.LevelRecording;
            levelStats.LevelRecording = null;

            
            string serialized = JsonUtility.ToJson(levelStats);
            string compressed = serialized.Compress();

            string key = PersistentDataKeys.LevelStats_Old(levelName);
            
            PersistentDataHelper.SetString(key, compressed, true);

            levelStats.LevelRecording = notSavedLevelRecording;
            levelStats.FastestLevelRecording = null;
            levelStats.FastestPerfectLevelRecording = null;

            CircumDebug.Log($"Saved level stats for using old method for {levelName}");
#pragma warning restore 612
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
            
#pragma warning disable 612
            // allow using obsolete data functions here for backwards compatibility sake
            string oldKey = PersistentDataKeys.LevelStats_Old(levelName);
            string perfectRecordingKey = PersistentDataKeys.PerfectLevelRecording(levelName);
            string imperfectRecordingKey = PersistentDataKeys.ImperfectLevelRecording(levelName);
#pragma warning restore 612
            
            string metaKey = PersistentDataKeys.LevelMetaStats(levelName);
            string levelRecordingKey = PersistentDataKeys.LevelRecording(levelName);
            
            PersistentDataHelper.DeleteKey(oldKey);
            PersistentDataHelper.DeleteKey(metaKey);
            PersistentDataHelper.DeleteKey(perfectRecordingKey);
            PersistentDataHelper.DeleteKey(imperfectRecordingKey);
            PersistentDataHelper.DeleteKey(levelRecordingKey);
        }

        public void SetRecordingFromOldData(LevelRecording oldRecording, LevelRecording oldPerfectRecording, float goldTime)
        {
            if (LevelRecordingExists(oldPerfectRecording))
            {
                LevelRecording = oldPerfectRecording;
                LevelRecording.RecordingData.IsPerfect = true;
                HasPreviouslyCompletedInGoldTime = LevelRecording.HasBeatenGoldTime(goldTime);
                return;
            }

            if (LevelRecordingExists(oldRecording))
            {
                LevelRecording = oldRecording;
                LevelRecording.RecordingData.IsPerfect = false;
            }
        }
    }
}