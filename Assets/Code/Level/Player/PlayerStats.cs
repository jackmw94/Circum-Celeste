using System;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class PlayerStats
    {
        private const string PlayerPrefsKey = "Circum_PlayerStats";

        [SerializeField] private bool _completedTutorials;
        [SerializeField] private int _lastLevelPlayed = 0;
        [SerializeField] private int _highestLevelReached = 0;
        [SerializeField] private int _highestNoDeathLevelReached = 0;
        [SerializeField] private int _highestPerfectLevelReached = 0;

        public bool CompletedTutorials => _completedTutorials;
        public int LastLevelPlayed => _lastLevelPlayed;
        public int HighestLevel => _highestLevelReached;
        public int HighestLevelNoDeaths => _highestNoDeathLevelReached;
        public int HighestPerfectLevel => _highestPerfectLevelReached;

        public void SetLastLevelPlayed(int level)
        {
            CircumDebug.Log($"Setting last level played {level} (user facing id)");
            _lastLevelPlayed = level;
        }

        public void UpdateHighestLevel(int level, bool noDeaths, bool noHits)
        {
            _highestLevelReached = Mathf.Max(level, _highestLevelReached);

            if (noDeaths)
            {
                _highestNoDeathLevelReached = Mathf.Max(level, _highestNoDeathLevelReached);
            }
            
            if (noHits)
            {
                _highestPerfectLevelReached = Mathf.Max(level, _highestPerfectLevelReached);
            }
        }

        public void UpdateCompletedTutorials(bool hasCompletedTutorials, bool forceSet = false)
        {
            if (forceSet)
            {
                _completedTutorials = hasCompletedTutorials;
            }
            else
            {
                _completedTutorials |= hasCompletedTutorials;
            }
        }

        public override string ToString()
        {
            return $"Tutorials complete = {_completedTutorials}, highest level reached = {_highestLevelReached}, highest level with no deaths = {_highestNoDeathLevelReached}, highest level on perfect run = {_highestPerfectLevelReached}";
        }

        public static void Save(PlayerStats stats)
        {
            string serialized = JsonUtility.ToJson(stats);
            PlayerPrefs.SetString(PlayerPrefsKey, serialized);
            PlayerPrefs.Save();
            CircumDebug.Log($"Saved player stats: {stats}");
        }
        
        public static PlayerStats Load()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                CircumDebug.Log("Created new player stats since we can't find saved key");
                return new PlayerStats();
            }
            
            string serializedPlayerStats = PlayerPrefs.GetString(PlayerPrefsKey);
            PlayerStats deserializedPlayerStats = JsonUtility.FromJson<PlayerStats>(serializedPlayerStats);
            CircumDebug.Log($"Loaded player stats: {deserializedPlayerStats}");
            return deserializedPlayerStats;
        }
        
        public static void ResetSavedPlayerStats()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKey);
        }
    }
}