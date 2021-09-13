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
        private const float DefaultMoverUIRelativeMovementSensitivity = 1;
        private const float DefaultMoverUIRelativeMovementRatio = 0.5f;
        private const float DefaultOrbiterP = 0.04f;
        private const float DefaultOrbiterI = 0.325f;
        private const float DefaultOrbiterD = 0f;
        private const float DefaultOrbiter9x9P = 0.04f;
        private const float DefaultOrbiter9x9I = 0.375f;
        private const float DefaultOrbiter11x11P = 0.04f;
        private const float DefaultOrbiter11x11I = 0.35f;
        private const float DefaultOrbiter13x13P = 0.04f;
        private const float DefaultOrbiter13x13I = 0.325f;
        private const float DefaultPickupColliderSize = 0.35f;
        private const float DefaultHazardColliderSize = 0.35f;
        private const float DefaultPlayerSpeed = 5f;
        private const float DefaultSlingIntegralOffset = 0.7f;
        private const float DefaultSlingProportionalOffset = -0.2f;
        private const float DefaultMoverDeadZone = 0.2f;
        private const float DefaultLevelSizeSpeedAdjustmentFactor = 0.5f;
        
        [Serializable]
        public struct UserAttributes { }
        
        [Serializable]
        public struct AppAttributes { }
        
        private static Action<bool> _requestRefreshCallback = (success) => { };
        public static Action RemoteConfigUpdated = () => { };

        // Config values
        public static float EnemyColliderRadius = DefaultEnemyColliderRadius;
        public static bool MoverUIRelative = DefaultMoverUIRelative;
        public static float MoverUIRelativeMovementSensitivity = DefaultMoverUIRelativeMovementSensitivity;
        public static float MoverUIRelativeMovementRatio = DefaultMoverUIRelativeMovementRatio;
        public static float OrbiterD = DefaultOrbiterD;
        public static float OrbiterI = DefaultOrbiterI;
        public static float OrbiterP = DefaultOrbiterP;
        public static float OrbiterP9x9 = DefaultOrbiter9x9P;
        public static float OrbiterI9x9 = DefaultOrbiter9x9I;
        public static float OrbiterP11x11 = DefaultOrbiter11x11P;
        public static float OrbiterI11x11 = DefaultOrbiter11x11I;
        public static float OrbiterP13x13 = DefaultOrbiter13x13P;
        public static float OrbiterI13x13 = DefaultOrbiter13x13I;
        public static float PickupColliderSize = DefaultPickupColliderSize;
        public static float HazardColliderSize = DefaultHazardColliderSize;
        public static float PlayerSpeed = DefaultPlayerSpeed;
        public static float SlingIntegralOffset = DefaultSlingIntegralOffset;
        public static float SlingProportionalOffset = DefaultSlingProportionalOffset;
        public static string FeedbackProperties = "";
        public static float MoverDeadZone = DefaultMoverDeadZone;
        public static float LevelSizeSpeedAdjustmentFactor = DefaultLevelSizeSpeedAdjustmentFactor;
        //

        private static Vector3 OrbiterPidValues9x9 => new Vector3(OrbiterP9x9, OrbiterI9x9, 0f);
        private static Vector3 OrbiterPidValues11x11 => new Vector3(OrbiterP11x11, OrbiterI11x11, 0f);
        private static Vector3 OrbiterPidValues13x13 => new Vector3(OrbiterP13x13, OrbiterI13x13, 0f);

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
                CircumDebug.LogError($"Could not update game config : {configResponse.status.ToString()}");
                LoadConfigFromPlayerPrefs();
            }

            _requestRefreshCallback?.Invoke(success);
        }

        private static void SetValuesFromRemoteConfig()
        {
            AssertConfigHasProperties();
            
            EnemyColliderRadius = ConfigManager.appConfig.GetFloat(nameof(EnemyColliderRadius), DefaultEnemyColliderRadius);
            MoverUIRelative = ConfigManager.appConfig.GetBool(nameof(MoverUIRelative));
            MoverUIRelativeMovementSensitivity = ConfigManager.appConfig.GetFloat(nameof(MoverUIRelativeMovementSensitivity));
            MoverUIRelativeMovementRatio = ConfigManager.appConfig.GetFloat(nameof(MoverUIRelativeMovementRatio));
            OrbiterD = ConfigManager.appConfig.GetFloat(nameof(OrbiterD));
            OrbiterI = ConfigManager.appConfig.GetFloat(nameof(OrbiterI), DefaultOrbiterI);
            OrbiterP = ConfigManager.appConfig.GetFloat(nameof(OrbiterP), DefaultOrbiterP);
            PickupColliderSize = ConfigManager.appConfig.GetFloat(nameof(PickupColliderSize), DefaultPickupColliderSize);
            HazardColliderSize = ConfigManager.appConfig.GetFloat(nameof(HazardColliderSize), DefaultHazardColliderSize);
            PlayerSpeed = ConfigManager.appConfig.GetFloat(nameof(PlayerSpeed), DefaultPlayerSpeed);
            SlingIntegralOffset = ConfigManager.appConfig.GetFloat(nameof(SlingIntegralOffset), DefaultSlingIntegralOffset);
            SlingProportionalOffset = ConfigManager.appConfig.GetFloat(nameof(SlingProportionalOffset), DefaultSlingProportionalOffset);
            FeedbackProperties = ConfigManager.appConfig.GetString(nameof(FeedbackProperties));
            MoverDeadZone = ConfigManager.appConfig.GetFloat(nameof(MoverDeadZone));
            LevelSizeSpeedAdjustmentFactor = ConfigManager.appConfig.GetFloat(nameof(LevelSizeSpeedAdjustmentFactor));
            OrbiterP9x9 = ConfigManager.appConfig.GetFloat(nameof(OrbiterP9x9));
            OrbiterI9x9 = ConfigManager.appConfig.GetFloat(nameof(OrbiterI9x9));
            OrbiterP11x11 = ConfigManager.appConfig.GetFloat(nameof(OrbiterP11x11));
            OrbiterI11x11 = ConfigManager.appConfig.GetFloat(nameof(OrbiterI11x11));
            OrbiterP13x13 = ConfigManager.appConfig.GetFloat(nameof(OrbiterP13x13));
            OrbiterI13x13 = ConfigManager.appConfig.GetFloat(nameof(OrbiterI13x13));
        }

        private static void AssertConfigHasProperties()
        {
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(SlingProportionalOffset)),$"There is no app config property for {nameof(SlingProportionalOffset)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(MoverUIRelative)),$"There is no app config property for {nameof(MoverUIRelative)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(MoverUIRelativeMovementSensitivity)),$"There is no app config property for {nameof(MoverUIRelativeMovementSensitivity)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(MoverUIRelativeMovementRatio)),$"There is no app config property for {nameof(MoverUIRelativeMovementRatio)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterD)),$"There is no app config property for {nameof(OrbiterD)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterI)),$"There is no app config property for {nameof(OrbiterI)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterP)),$"There is no app config property for {nameof(OrbiterP)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(PickupColliderSize)),$"There is no app config property for {nameof(PickupColliderSize)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(HazardColliderSize)),$"There is no app config property for {nameof(HazardColliderSize)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(PlayerSpeed)),$"There is no app config property for {nameof(PlayerSpeed)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(SlingIntegralOffset)),$"There is no app config property for {nameof(SlingIntegralOffset)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(SlingProportionalOffset)),$"There is no app config property for {nameof(SlingProportionalOffset)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(FeedbackProperties)),$"There is no app config property for {nameof(FeedbackProperties)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(MoverDeadZone)),$"There is no app config property for {nameof(MoverDeadZone)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(LevelSizeSpeedAdjustmentFactor)),$"There is no app config property for {nameof(LevelSizeSpeedAdjustmentFactor)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterP9x9)),$"There is no app config property for {nameof(OrbiterP9x9)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterI9x9)),$"There is no app config property for {nameof(OrbiterI9x9)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterP11x11)),$"There is no app config property for {nameof(OrbiterP11x11)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterI11x11)),$"There is no app config property for {nameof(OrbiterI11x11)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterP13x13)),$"There is no app config property for {nameof(OrbiterP13x13)}");
            CircumDebug.Assert(ConfigManager.appConfig.HasKey(nameof(OrbiterI13x13)),$"There is no app config property for {nameof(OrbiterI13x13)}");
        }

        private static void SaveConfigToPlayerPrefs()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(EnemyColliderRadius)), EnemyColliderRadius);
            PlayerPrefs.SetInt(PlayerPrefsKeyFromName(nameof(MoverUIRelative)), MoverUIRelative ? 1 : 0);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(MoverUIRelativeMovementSensitivity)), MoverUIRelativeMovementSensitivity);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(MoverUIRelativeMovementRatio)), MoverUIRelativeMovementRatio);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterD)), OrbiterD);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI)), OrbiterI);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP)), OrbiterP);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(PickupColliderSize)), PickupColliderSize);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(HazardColliderSize)), HazardColliderSize);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(PlayerSpeed)), PlayerSpeed);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(SlingIntegralOffset)), SlingIntegralOffset);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(SlingProportionalOffset)), SlingProportionalOffset);
            PlayerPrefs.SetString(PlayerPrefsKeyFromName(nameof(FeedbackProperties)), FeedbackProperties);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(MoverDeadZone)), MoverDeadZone);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(LevelSizeSpeedAdjustmentFactor)), LevelSizeSpeedAdjustmentFactor);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP9x9)), OrbiterP9x9);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI9x9)), OrbiterI9x9);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP11x11)), OrbiterP11x11);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI11x11)), OrbiterI11x11);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP13x13)), OrbiterP13x13);
            PlayerPrefs.SetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI13x13)), OrbiterI13x13);
        }

        private static void LoadConfigFromPlayerPrefs()
        {
            EnemyColliderRadius = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(EnemyColliderRadius)), DefaultEnemyColliderRadius);
            MoverUIRelative = PlayerPrefs.GetInt(PlayerPrefsKeyFromName(nameof(MoverUIRelative)), DefaultMoverUIRelative ? 1 : 0) == 1;
            MoverUIRelativeMovementSensitivity = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(MoverUIRelativeMovementSensitivity)), DefaultMoverUIRelativeMovementSensitivity);
            MoverUIRelativeMovementSensitivity = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(MoverUIRelativeMovementRatio)), DefaultMoverUIRelativeMovementRatio);
            OrbiterD = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterD)), DefaultOrbiterD);
            OrbiterI = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI)), DefaultOrbiterI);
            OrbiterP = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP)), DefaultOrbiterP);
            PickupColliderSize = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(PickupColliderSize)), DefaultPickupColliderSize);
            HazardColliderSize = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(HazardColliderSize)), DefaultHazardColliderSize);
            PlayerSpeed = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(PlayerSpeed)), DefaultPlayerSpeed);
            SlingIntegralOffset = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(SlingIntegralOffset)), DefaultSlingIntegralOffset);
            SlingProportionalOffset = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(SlingProportionalOffset)), DefaultSlingIntegralOffset);
            FeedbackProperties = PlayerPrefs.GetString(PlayerPrefsKeyFromName(nameof(FeedbackProperties)), "");
            MoverDeadZone = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(MoverDeadZone)), DefaultMoverDeadZone);
            LevelSizeSpeedAdjustmentFactor = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(LevelSizeSpeedAdjustmentFactor)), DefaultLevelSizeSpeedAdjustmentFactor);
            OrbiterP9x9 = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP9x9)), DefaultOrbiter9x9P);
            OrbiterI9x9 = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI9x9)), DefaultOrbiter9x9I);
            OrbiterP11x11 = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP11x11)), DefaultOrbiter11x11P);
            OrbiterI11x11 = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI11x11)), DefaultOrbiter11x11I);
            OrbiterP13x13 = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterP13x13)), DefaultOrbiter13x13P);
            OrbiterI13x13 = PlayerPrefs.GetFloat(PlayerPrefsKeyFromName(nameof(OrbiterI13x13)), DefaultOrbiter13x13I);
        }

        private static string PlayerPrefsKeyFromName(string propertyName)
        {
            return $"Circum_Config_{propertyName}";
        }

        public static Vector3 GetOrbiterPidValuesFromGridSize(int gridSize)
        {
            return gridSize switch
            {
                9 => OrbiterPidValues9x9,
                11 => OrbiterPidValues11x11,
                13 => OrbiterPidValues13x13,
                _ => throw new ArgumentOutOfRangeException($"Could not find orbiter value for grid size {gridSize}")
            };
        }
    }
}