using System;
using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class CircumOptions
    {
        [field: SerializeField] public bool ShowLevelTimer { get; set; } = false;

        public static CircumOptions Load()
        {
            if (!CircumPlayerPrefs.HasKey(PlayerPrefsKeys.CircumOptions))
            {
                return new CircumOptions();
            }

            string serializedOptions = CircumPlayerPrefs.GetString(PlayerPrefsKeys.CircumOptions);
            CircumDebug.Log($"Loaded circum options {serializedOptions}");
            
            CircumOptions deserializedOptions = JsonUtility.FromJson<CircumOptions>(serializedOptions);

            if (deserializedOptions == null)
            {
                CircumDebug.LogError("Loaded options were null, creating new.");
                return new CircumOptions();
            }
            
            return deserializedOptions;
        }

        public static void Save(CircumOptions circumOptions)
        {
            string serializedCircumOptions = JsonUtility.ToJson(circumOptions);
            
            CircumPlayerPrefs.SetString(PlayerPrefsKeys.CircumOptions, serializedCircumOptions);
            
            CircumDebug.Log($"Saved circum options : {serializedCircumOptions}");
        }

        public static void ResetOptions()
        {
            CircumPlayerPrefs.DeleteKey(PlayerPrefsKeys.CircumOptions);
        }
    }
}