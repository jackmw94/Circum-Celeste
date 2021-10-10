using Code.Core;
using Code.Debugging;
using UnityExtras.Code.Core;

namespace Code.Level.Player
{
    public static class PersistentDataHelper
    {
        private static IPlayerPrefsProvider _playerPrefsProvider;

        private static string LongKey_FirstInt(string key) => $"{key}_int1";
        private static string LongKey_SecondInt(string key) => $"{key}_int2";
        
        static PersistentDataHelper()
        {
            _playerPrefsProvider = new SynchronousPlayerPrefs();
        }

        public static bool TryGetLong(string key, out long longValue)
        {
            string firstIntKey = LongKey_FirstInt(key);
            string secondIntKey = LongKey_SecondInt(key);

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

            string firstIntKey = LongKey_FirstInt(key);
            string secondIntKey = LongKey_SecondInt(key);
            
            SetInt(firstIntKey, first);
            SetInt(secondIntKey, second);
        }

        public static void SetInt(string key, int value) => _playerPrefsProvider.SetInt(key, value);

        public static void SetFloat(string key, float value) => _playerPrefsProvider.SetFloat(key, value);

        public static bool HasKey(string key) => _playerPrefsProvider.HasKey(key);

        public static void SetString(string key, string value, bool saveRemote)
        {
            _playerPrefsProvider.SetString(key, value);

            if (saveRemote)
            {
                if (RemoteDataManager.Instance.IsLoggedIn)
                {
                    RemoteDataManager.Instance.SetUserData(key, value);
                }
                else
                {
                    CircumDebug.LogError($"Could not save {key} remotely because we're not logged in!");
                }
            }
        }

        public static void Save() => _playerPrefsProvider.Save();

        public static void DeleteKey(string key, bool isLong = false)
        {
            if (!isLong)
            {
                _playerPrefsProvider.DeleteKey(key);
            }
            else
            {
                _playerPrefsProvider.DeleteKey(LongKey_FirstInt(key));
                _playerPrefsProvider.DeleteKey(LongKey_SecondInt(key));
            }
        }

        public static void ResetSaveData() => _playerPrefsProvider.ResetSaveData();
    }
}