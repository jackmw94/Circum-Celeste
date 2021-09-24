using System;
using UnityExtras.Code.Core;

namespace Code.Level.Player
{
    public static class CircumPlayerPrefs
    {
        private static IPlayerPrefsProvider _playerPrefsProvider;

        private static string FirstIntKey(string key) => $"{key}_int1";
        private static string SecondIntKey(string key) => $"{key}_int2";
        
        static CircumPlayerPrefs()
        {
#if CONCURRENT_PLAYER_PREFS
            _playerPrefsProvider = new ConcurrentPlayerPrefs();
#else
            _playerPrefsProvider = new SynchronousPlayerPrefs();
#endif
        }

        public static bool TryGetLong(string key, out long longValue)
        {
            string firstIntKey = FirstIntKey(key);
            string secondIntKey = SecondIntKey(key);

            if (!HasKey(firstIntKey) || !HasKey(secondIntKey))
            {
                longValue = 0;
                return false;
            }
            
            int firstInt = _playerPrefsProvider.GetInt(firstIntKey);
            int secondInt = _playerPrefsProvider.GetInt(secondIntKey);

            longValue =  Utilities.ConstructLongFromInts(firstInt, secondInt);
            return true;
        }

        public static int GetInt(string key, int defaultValue = 0) => _playerPrefsProvider.GetInt(key, defaultValue);

        public static float GetFloat(string key, float defaultValue = 0.0f) => _playerPrefsProvider.GetFloat(key, defaultValue);

        public static string GetString(string key, string defaultValue = "") => _playerPrefsProvider.GetString(key, defaultValue);

        public static void SetLong(string key, long longValue)
        {
            (int first, int second) = Utilities.DeconstructLongToInts(longValue);

            string firstIntKey = FirstIntKey(key);
            string secondIntKey = SecondIntKey(key);
            
            SetInt(firstIntKey, first);
            SetInt(secondIntKey, second);
        }

        public static void SetInt(string key, int value) => _playerPrefsProvider.SetInt(key, value);

        public static void SetFloat(string key, float value) => _playerPrefsProvider.SetFloat(key, value);

        public static bool HasKey(string key) => _playerPrefsProvider.HasKey(key);

        public static void SetString(string key, string value) => _playerPrefsProvider.SetString(key, value);

        public static void Save() => _playerPrefsProvider.Save();

        public static void DeleteKey(string key, bool isLong = false)
        {
            if (!isLong)
            {
                _playerPrefsProvider.DeleteKey(key);
            }
            else
            {
                _playerPrefsProvider.DeleteKey(FirstIntKey(key));
                _playerPrefsProvider.DeleteKey(SecondIntKey(key));
            }
        }

        public static void ResetSaveData() => _playerPrefsProvider.ResetSaveData();
    }
}