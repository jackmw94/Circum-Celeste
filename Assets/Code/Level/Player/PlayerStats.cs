using System;
using Code.Debugging;
using UnityEditor;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class PlayerStats
    {
        private const string PlayerPrefsKey = "Circum_PlayerStats";
        
        [SerializeField] private int _highestLevelReached = 0;
        [SerializeField] private int _highestNoDeathLevelReached = 0;
        [SerializeField] private int _highestPerfectLevelReached = 0;

        public int HighestLevel => _highestLevelReached;
        public int HighestLevelNoDeaths => _highestNoDeathLevelReached;
        public int HighestPerfectLevel => _highestPerfectLevelReached;

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
            
            Save(this);
        }

        public override string ToString()
        {
            return $"Highest level reached = {_highestLevelReached}, highest level with no deaths = {_highestNoDeathLevelReached}, highest level on perfect run = {_highestPerfectLevelReached}";
        }

        public static void Save(PlayerStats stats)
        {
            string serialized = JsonUtility.ToJson(stats);
            PlayerPrefs.SetString(PlayerPrefsKey, serialized);
        }
        
        public static PlayerStats Load()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                return new PlayerStats();
            }
            
            string serializedPlayerStats = PlayerPrefs.GetString(PlayerPrefsKey);
            PlayerStats deserializedPlayerStats = JsonUtility.FromJson<PlayerStats>(serializedPlayerStats);
            return deserializedPlayerStats;
        }
        
#if UNITY_EDITOR
        [MenuItem("Circum/Reset saved player stats")]
#endif
        public static void ResetSavedPlayerStats()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKey);
        }
    }
}