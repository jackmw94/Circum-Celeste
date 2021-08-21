using System;
using Code.Debugging;
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
        private const float DefaultSlingIncrease = 1f;
        
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
        public static float SlingIncrease = DefaultSlingIncrease;
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
            EnemyColliderRadius = ConfigManager.appConfig.GetFloat("EnemyColliderRadius", DefaultEnemyColliderRadius);
            MoverUIRelative = ConfigManager.appConfig.GetBool("MoverUIRelative", DefaultMoverUIRelative);
            OrbiterD = ConfigManager.appConfig.GetFloat("OrbiterD", DefaultOrbiterD);
            OrbiterI = ConfigManager.appConfig.GetFloat("OrbiterI", DefaultOrbiterI);
            OrbiterP = ConfigManager.appConfig.GetFloat("OrbiterP", DefaultOrbiterP);
            PickupColliderSize = ConfigManager.appConfig.GetFloat("PickupColliderSize", DefaultPickupColliderSize);
            PlayerSpeed = ConfigManager.appConfig.GetFloat("PlayerSpeed", DefaultPlayerSpeed);
            SlingIncrease = ConfigManager.appConfig.GetFloat("SlingIncrease", DefaultSlingIncrease);
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
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(SlingIncrease)), SlingIncrease);
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
            SlingIncrease = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(SlingIncrease)), DefaultSlingIncrease);
        }

        private static string PlayerPrefsKeyFromName(string propertyName)
        {
            return $"Circum_Config_{propertyName}";
        }
    }
}