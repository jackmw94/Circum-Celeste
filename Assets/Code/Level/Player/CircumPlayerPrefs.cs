using System;

namespace Code.Level.Player
{
    public static class CircumPlayerPrefs
    {
        private static IPlayerPrefsProvider _playerPrefsProvider;

        static CircumPlayerPrefs()
        {
#if CONCURRENT_PLAYER_PREFS
            _playerPrefsProvider = new ConcurrentPlayerPrefs();
#else
            _playerPrefsProvider = new SynchronousPlayerPrefs();
#endif
        }
        
        public static int GetInt(string key, int defaultValue = 0) => _playerPrefsProvider.GetInt(key, defaultValue);

        public static float GetFloat(string key, float defaultValue = 0.0f) => _playerPrefsProvider.GetFloat(key, defaultValue);

        public static string GetString(string key, string defaultValue = "") => _playerPrefsProvider.GetString(key, defaultValue);

        public static void SetInt(string key, int value)
        {
            _playerPrefsProvider.SetInt(key, value);
        }

        public static void SetFloat(string key, float value)
        {
            _playerPrefsProvider.SetFloat(key, value);
        }

        public static bool HasKey(string key)
        {
            return _playerPrefsProvider.HasKey(key);
        }

        public static void SetString(string key, string value)
        {
            _playerPrefsProvider.SetString(key, value);
        }

        public static void Save()
        {
            _playerPrefsProvider.Save();
        }
        
        public static void DeleteKey(string key)
        {
            _playerPrefsProvider.DeleteKey(key);
        }

        public static void DeleteAll()
        {
            throw new NotImplementedException("Did not anticipate deleting all player prefs in this code, this is quite a destructive operation. Comment this exception to enable");
            
            _playerPrefsProvider.DeleteAll();
        }

        public static void ResetSaveData()
        {
            _playerPrefsProvider.ResetSaveData();
        }
    }
}