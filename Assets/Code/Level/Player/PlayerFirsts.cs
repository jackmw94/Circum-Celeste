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
        public const int TutorialDeathsUntilHowToPlaySuggested = 2;
        
        [field: SerializeField] public bool SuggestedHowToPlay { get; set; } = false;
        [field: SerializeField] public bool SeenReplaysScreen { get; set; } = false;
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

        public void ShowReplayScreenIntroIfFirst()
        {
            if (SeenReplaysScreen)
            {
                return;
            }
            
            // show intro
            
            PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
            persistentDataManager.PlayerFirsts.SeenReplaysScreen = true;
            
            Save(persistentDataManager.PlayerFirsts);
        }

        public void SetReplaysScreenAsSeen()
        {
            PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
            
            persistentDataManager.PlayerFirsts.SeenReplaysScreen = true;
            Save(persistentDataManager.PlayerFirsts);
            
            CircumDebug.Log("Set replays screen as seen");
        }

        public static PlayerFirsts Load()
        {
            if (!PersistentDataHelper.HasKey(PlayerPrefsKeys.PlayerFirsts))
            {
                return new PlayerFirsts();
            }

            string serializedPlayerFirsts = PersistentDataHelper.GetString(PlayerPrefsKeys.PlayerFirsts);
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
            PersistentDataHelper.SetString(PlayerPrefsKeys.PlayerFirsts, serializedPlayerFirsts, false);
            
            CircumDebug.Log($"Saved player firsts : {serializedPlayerFirsts}");
        }

        public static void ResetPlayerFirsts()
        {
            PersistentDataHelper.DeleteKey(PlayerPrefsKeys.PlayerFirsts);
        }
    }
}