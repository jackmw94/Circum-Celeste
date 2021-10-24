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
        [field: SerializeField] public bool SeenNewFriendPopUp { get; set; } = false;
        [field: SerializeField] public bool UsedLevelOverviewArrows { get; set; } = false;

        public void ShowHowToPlayPopUpIfFirst()
        {
            if (SuggestedHowToPlay)
            {
                return;
            }
            
            Popup.Instance.EnqueueMessage(Popup.LocalisedPopupType.SeeHowToPlay);
            
            SuggestedHowToPlay = true;
            
            Save(this);
        }

        public void ShowReplayScreenIntroIfFirst()
        {
            if (SeenReplaysScreen)
            {
                return;
            }
            
            SeenReplaysScreen = true;
            
            Save(this);
        }

        public void SetLevelOverviewArrowsUsed()
        {
            if (UsedLevelOverviewArrows)
            {
                return;
            }

            UsedLevelOverviewArrows = true;
            
            Save(this);
        }

        public void ShowNewFriendPopupIfFirst()
        {
            if (SeenNewFriendPopUp)
            {
                return;
            }
            
            Popup.Instance.EnqueueMessage(Popup.LocalisedPopupType.YouHaveNewFriend);

            SeenNewFriendPopUp = true;
            
            Save(this);
        }

        public void SetReplaysScreenAsSeen()
        {
            SeenReplaysScreen = true;
            
            Save(this);
            CircumDebug.Log("Set replays screen as seen");
        }

        public static PlayerFirsts Load()
        {
            if (!PersistentDataHelper.HasKey(PersistentDataKeys.PlayerFirsts))
            {
                return new PlayerFirsts();
            }

            string serializedPlayerFirsts = PersistentDataHelper.GetString(PersistentDataKeys.PlayerFirsts);
            CircumDebug.Log($"Loaded player firsts {serializedPlayerFirsts}");
            
            PlayerFirsts deserializedPlayerFirsts = JsonUtility.FromJson<PlayerFirsts>(serializedPlayerFirsts);
            
            if (deserializedPlayerFirsts != null)
            {
                return deserializedPlayerFirsts;
            }
            
            CircumDebug.LogError("Loaded player firsts were null, creating new.");
            return new PlayerFirsts();
        }

        public static void Save(PlayerFirsts playerFirsts)
        {
            string serializedPlayerFirsts = JsonUtility.ToJson(playerFirsts);
            PersistentDataHelper.SetString(PersistentDataKeys.PlayerFirsts, serializedPlayerFirsts, false);
            
            CircumDebug.Log($"Saved player firsts : {serializedPlayerFirsts}");
        }

        public static void ResetPlayerFirsts()
        {
            PersistentDataHelper.DeleteKey(PersistentDataKeys.PlayerFirsts);
        }
    }
}