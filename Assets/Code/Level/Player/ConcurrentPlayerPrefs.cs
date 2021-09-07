using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Level.Player
{
    public class ConcurrentPlayerPrefs : IPlayerPrefsProvider
    {
        private ConcurrentDictionary<string, object> _playerPrefsDictionary = new ConcurrentDictionary<string, object>();
        private readonly object _locker = new object();

#if !UNITY_EDITOR && UNITY_ANDROID
        public const string SavesDirectory = @"sdcard/Circum/";
        public const string SavesFile = SavesDirectory + @"/save-file";
#elif !UNITY_EDITOR && UNITY_IOS
        public static string SavesDirectory => Application.persistentDataPath;
        public static string SavesFile => SavesDirectory + @"/save-file";
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
        public const string SavesDirectory = "C:\\Users\\Demo\\Documents\\Circum\\";
        public const string SavesFile = SavesDirectory + "save-file.txt";
#endif

        public ConcurrentPlayerPrefs()
        {
            Load();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return (int) _playerPrefsDictionary.GetOrAdd(key, defaultValue);
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            return (float) _playerPrefsDictionary.GetOrAdd(key, defaultValue);
        }

        public string GetString(string key, string defaultValue = "")
        {
            return (string) _playerPrefsDictionary.GetOrAdd(key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            _playerPrefsDictionary.AddOrUpdate(key, value, (currentKey, currentValue) => value);
        }

        public void SetFloat(string key, float value)
        {
            _playerPrefsDictionary.AddOrUpdate(key, value, (currentKey, currentValue) => value);
        }

        public void SetString(string key, string value)
        {
            Thread thread = new Thread(SetStringInternal);
            thread.Start((key, value));
        }

        private void SetStringInternal(object o)
        {
            (string key, string value) = ((string, string)) o;
            _playerPrefsDictionary.AddOrUpdate(key, value, (currentKey, currentValue) => value);
        }

        public void Save()
        {
            Thread thread = new Thread(SaveInternal);
            thread.Start();
        }

        private void SaveInternal()
        {
            lock (_locker)
            {
                File.WriteAllText(SavesFile, JsonConvert.SerializeObject(_playerPrefsDictionary));
            }
        }

        private void Load()
        {
            if (!Directory.Exists(SavesDirectory))
            {
                Directory.CreateDirectory(SavesDirectory);
            }

            if (!File.Exists(SavesFile))
            {
                File.Create(SavesFile);
            }
            else
            {
                _playerPrefsDictionary = JsonConvert.DeserializeObject<ConcurrentDictionary<string, object>>(File.ReadAllText(SavesFile));
            }
        }

        public void DeleteKey(string key)
        {
            lock (_locker)
            {
                _playerPrefsDictionary.TryRemove(key, out object _);
            }
        }

        public void DeleteAll()
        {
            lock (_locker)
            {
                _playerPrefsDictionary.Clear();
            }
        }
    }
}