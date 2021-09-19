using System;
using Code.Core;
using Code.Debugging;
using Code.UI;
using UnityEngine;

namespace Code.Level.Player
{
    [Serializable]
    public class PlayerFirsts
    {
        public const int TutorialDeathsUntilHowToPlaySuggested = 3;
        
        [field: SerializeField] public bool SuggestedHowToPlay { get; set; } = false;
        [field: SerializeField] public bool CompletedGame { get; set; } = false;

        public void ShowHowToPlayPopUpIfFirst()
        {
            if (SuggestedHowToPlay)
            {
                return;
            }
            
            Popup.Instance.EnqueueMessage(Popup.LocalisedPopupType.SeeHowToPlay);
            
            PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
            persistentDataManager.PlayerFirsts.SuggestedHowToPlay = true;
            
            Save(persistentDataManager.PlayerFirsts);
        }

        public static PlayerFirsts Load()
        {
            if (!CircumPlayerPrefs.HasKey(PlayerPrefsKeys.PlayerFirsts))
            {
                return new PlayerFirsts();
            }

            string serializedPlayerFirsts = CircumPlayerPrefs.GetString(PlayerPrefsKeys.PlayerFirsts);
            CircumDebug.Log($"Loaded player firsts {serializedPlayerFirsts}");
            
            PlayerFirsts deserializedPlayerFirsts = JsonUtility.FromJson<PlayerFirsts>(serializedPlayerFirsts);

            if (deserializedPlayerFirsts == null)
            {
                CircumDebug.LogError("Loaded player firsts were null, creating new.");
                return new PlayerFirsts();
            }
            
            return deserializedPlayerFirsts;
        }

        public static void Save(PlayerFirsts playerFirsts)
        {
            string serializedPlayerFirsts = JsonUtility.ToJson(playerFirsts);
            CircumPlayerPrefs.SetString(PlayerPrefsKeys.PlayerFirsts, serializedPlayerFirsts);
            
            CircumDebug.Log($"Saved player firsts : {serializedPlayerFirsts}");
        }

        public static void ResetPlayerFirsts()
        {
            CircumPlayerPrefs.DeleteKey(PlayerPrefsKeys.PlayerFirsts);
        }
    }
}