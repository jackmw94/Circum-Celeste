using System;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class CircumOptions
    {
        private const string CircumOptionsPlayerPrefsKey = "Circum_Options";

        [field: SerializeField] public bool ShowLevelTimer { get; set; } = false;

        public static CircumOptions Load()
        {
            if (!CircumPlayerPrefs.HasKey(CircumOptionsPlayerPrefsKey))
            {
                return new CircumOptions();
            }

            string serializedOptions = CircumPlayerPrefs.GetString(CircumOptionsPlayerPrefsKey);
            CircumDebug.Log($"Loaded circum options {serializedOptions}");
            
            CircumOptions deserializedOptions = JsonUtility.FromJson<CircumOptions>(serializedOptions);

            if (deserializedOptions == null)
            {
                CircumDebug.LogError("Loaded options were null, creating new.");
                return new CircumOptions();
            }
            
            return deserializedOptions;
        }

        public static void ResetOptions()
        {
            CircumPlayerPrefs.DeleteKey(CircumOptionsPlayerPrefsKey);
        }
    }
}