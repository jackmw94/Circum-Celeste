using System;
using System.Diagnostics;
using Code.Debugging;
using Code.Juice;
using Code.UI;
using Unity.RemoteConfig;
using UnityEngine;

namespace Code.Core
{
    public static class RemoteConfigHelper
    {
        private const float DefaultEnemyColliderRadius = 0.5f;
        private const bool DefaultMoverUIRelative = false;
        private const float DefaultOrbiterD = 0f;
        private const float DefaultOrbiterI = 0.3f;
        private const float DefaultOrbiterP = 0.01f;
        private const float DefaultPickupColliderSize = 0.35f;
        private const float DefaultPlayerSpeed = 5f;
        private const float DefaultSlingIntegralOffset = 0.7f;
        private const float DefaultSlingProportionalOffset = -0.2f;
        
        [Serializable]
        public struct UserAttributes { }
        
        [Serializable]
        public struct AppAttributes { }
        
        private static Action<bool> _requestRefreshCallback = (success) => { };
        public static Action RemoteConfigUpdated = () => { };

        // Config values
        public static float EnemyColliderRadius = DefaultEnemyColliderRadius;
        public static bool MoverUIRelative = DefaultMoverUIRelative;
        public static float OrbiterD = DefaultOrbiterD;
        public static float OrbiterI = DefaultOrbiterI;
        public static float OrbiterP = DefaultOrbiterP;
        public static float PickupColliderSize = DefaultPickupColliderSize;
        public static float PlayerSpeed = DefaultPlayerSpeed;
        public static float SlingIntegralOffset = DefaultSlingIntegralOffset;
        public static float SlingProportionalOffset = DefaultSlingProportionalOffset;
        //
        

        static RemoteConfigHelper()
        {
            ConfigManager.FetchCompleted += FetchCompleted;
        }

        public static void RequestRefresh(Action<bool> onComplete = null)
        {
            _requestRefreshCallback = onComplete;
            ConfigManager.FetchConfigs(new UserAttributes(), new AppAttributes());
        }

        private static void FetchCompleted(ConfigResponse configResponse)
        {
            bool success = configResponse.status == ConfigRequestStatus.Success;
            
            if (success)
            {
                CircumDebug.Log("Updated game config from server");
                SetValuesFromRemoteConfig();
                SaveConfigToPlayerPrefs();
                RemoteConfigUpdated();
            }
            else
            {
                CircumDebug.LogError($"Could not update game config : {configResponse.ToString()}");
                Popup.Instance.EnqueueMessage("Could not update game configuration");
                LoadConfigFromPlayerPrefs();
            }

            _requestRefreshCallback?.Invoke(success);
        }

        private static void SetValuesFromRemoteConfig()
        {
            AssertConfigHasProperties();
            
            EnemyColliderRadius = ConfigManager.appConfig.GetFloat(nameof(EnemyColliderRadius), DefaultEnemyColliderRadius);
            MoverUIRelative = ConfigManager.appConfig.GetBool(nameof(MoverUIRelative), DefaultMoverUIRelative);
            OrbiterD = ConfigManager.appConfig.GetFloat(nameof(OrbiterD), DefaultOrbiterD);
            OrbiterI = ConfigManager.appConfig.GetFloat(nameof(OrbiterI), DefaultOrbiterI);
            OrbiterP = ConfigManager.appConfig.GetFloat(nameof(OrbiterP), DefaultOrbiterP);
            PickupColliderSize = ConfigManager.appConfig.GetFloat(nameof(PickupColliderSize), DefaultPickupColliderSize);
            PlayerSpeed = ConfigManager.appConfig.GetFloat(nameof(PlayerSpeed), DefaultPlayerSpeed);
            SlingIntegralOffset = ConfigManager.appConfig.GetFloat(nameof(SlingIntegralOffset), DefaultSlingIntegralOffset);
            SlingProportionalOffset = ConfigManager.appConfig.GetFloat(nameof(SlingProportionalOffset), DefaultSlingProportionalOffset);
        }
        
        private static void AssertConfigHasProperties()
        {
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(SlingProportionalOffset)),$"There is no app config property for {nameof(SlingProportionalOffset)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(MoverUIRelative)),$"There is no app config property for {nameof(MoverUIRelative)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterD)),$"There is no app config property for {nameof(OrbiterD)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterI)),$"There is no app config property for {nameof(OrbiterI)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterP)),$"There is no app config property for {nameof(OrbiterP)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(PickupColliderSize)),$"There is no app config property for {nameof(PickupColliderSize)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(PlayerSpeed)),$"There is no app config property for {nameof(PlayerSpeed)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(SlingIntegralOffset)),$"There is no app config property for {nameof(SlingIntegralOffset)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(SlingProportionalOffset)),$"There is no app config property for {nameof(SlingProportionalOffset)}");
        }

        private static void SaveConfigToPlayerPrefs()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(EnemyColliderRadius)), EnemyColliderRadius);
            PlayerPrefs.SetInt(PlayerPrefsKeyFromName(nameof(MoverUIRelative)), MoverUIRelative ? 1 : 0);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterD)), OrbiterD);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI)), OrbiterI);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP)), OrbiterP);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(PickupColliderSize)), PickupColliderSize);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(PlayerSpeed)), PlayerSpeed);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(SlingIntegralOffset)), SlingIntegralOffset);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(SlingProportionalOffset)), SlingProportionalOffset);
        }

        private static void LoadConfigFromPlayerPrefs()
        {
            EnemyColliderRadius = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(EnemyColliderRadius)), DefaultEnemyColliderRadius);
            MoverUIRelative = PlayerPrefs.GetInt(PlayerPrefsKeyFromName(nameof(MoverUIRelative)), DefaultMoverUIRelative ? 1 : 0) == 1;
            OrbiterD = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterD)), DefaultOrbiterD);
            OrbiterI = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI)), DefaultOrbiterI);
            OrbiterP = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP)), DefaultOrbiterP);
            PickupColliderSize = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(PickupColliderSize)), DefaultPickupColliderSize);
            PlayerSpeed = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(PlayerSpeed)), DefaultPlayerSpeed);
            SlingIntegralOffset = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(SlingIntegralOffset)), DefaultSlingIntegralOffset);
            SlingProportionalOffset = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(SlingProportionalOffset)), DefaultSlingIntegralOffset);
        }

        private static string PlayerPrefsKeyFromName(string propertyName)
        {
            return $"Circum_Config_{propertyName}";
        }
    }
}