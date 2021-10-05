using System;
using Code.Level.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.UI
{
    public class FriendsLevelEntry : MonoBehaviour
    {
        public TextMeshProUGUI _friendsUsernameLabel;
        public TextMeshProUGUI _levelTimeLabel;
        public Image _perfectIcon;
        public Image _goldTimeIcon;
        public Image _perfectGoldIcon;
        public Button _replayButton;

        public void SetupEmptyRecord() => SetupRecord("", new BadgeData(), null, data => { });
        
        public void SetupRecord(string displayName, BadgeData badgeData, FriendsLevelRanking.FriendLevelData friendLevelData, Action<FriendsLevelRanking.FriendLevelData> replayCallback)
        {
            if (friendLevelData != null)
            {
                _friendsUsernameLabel.text = displayName;

                _perfectIcon.gameObject.SetActiveSafe(badgeData.IsPerfect && !badgeData.HasPerfectGoldTime);
                _goldTimeIcon.gameObject.SetActiveSafe(badgeData.HasGoldTime && !badgeData.HasPerfectGoldTime);
                _perfectGoldIcon.gameObject.SetActiveSafe(badgeData.HasPerfectGoldTime);
                    
                _levelTimeLabel.text = friendLevelData.Time.ToString("0.00");

                _replayButton.interactable = true;
                _replayButton.onClick.RemoveAllListeners();
                _replayButton.onClick.AddListener(() => { replayCallback(friendLevelData); });
            }
            else
            {
                _friendsUsernameLabel.text = "-";
                _levelTimeLabel.text = "[none]";
                _replayButton.interactable = false;
            }
        }
    }
}