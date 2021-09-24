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
        [SerializeField] private CircumQuality.CircumQualitySetting _circumQualitySetting = CircumQuality.CircumQualitySetting.High;
        public CircumQuality.CircumQualitySetting QualitySetting
        {
            get => _circumQualitySetting;
            set
            {
                _circumQualitySetting = value;
                CircumQuality.SetCircumQualityLevel(_circumQualitySetting);
            }
        }

        private CircumOptions() { }

        public static CircumOptions CreateCircumOptions()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer - can't use some unity API during serialization
            CircumOptions options = new CircumOptions();
            
            options.ShowLevelTimer = false;
            options.QualitySetting = CircumQuality.EstimateBestDefaultQualitySetting();

            return options;
        }

        public static CircumOptions Load()
        {
            if (!CircumPlayerPrefs.HasKey(PlayerPrefsKeys.CircumOptions))
            {
                return CreateCircumOptions();
            }

            string serializedOptions = CircumPlayerPrefs.GetString(PlayerPrefsKeys.CircumOptions);
            CircumDebug.Log($"Loaded circum options {serializedOptions}");
            
            CircumOptions deserializedOptions = JsonUtility.FromJson<CircumOptions>(serializedOptions);

            if (deserializedOptions == null)
            {
                CircumDebug.LogError("Loaded options were null, creating new.");
                return CreateCircumOptions();
            }
            
            // when we create new circum settings we set the quality level then, makes sense we do it when we load also
            CircumQuality.SetCircumQualityLevel(deserializedOptions.QualitySetting);
            
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

        public void SetNextQualitySetting()
        {
            int nextQualitySettingValue = (int) QualitySetting + 1;
            int numberOfQualitySettings = Enum.GetNames(typeof(CircumQuality.CircumQualitySetting)).Length;
            nextQualitySettingValue = nextQualitySettingValue % numberOfQualitySettings;
            
            QualitySetting = (CircumQuality.CircumQualitySetting) nextQualitySettingValue;
        }
    }
}