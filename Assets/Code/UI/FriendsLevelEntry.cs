﻿using System;
using Code.Level.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.UI
{
    public class FriendsLevelEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _friendsUsernameLabel;
        [SerializeField] private TextMeshProUGUI _levelTimeLabel;
        [SerializeField] private Image _perfectIcon;
        [SerializeField] private Image _goldTimeIcon;
        [SerializeField] private Image _perfectGoldIcon;
        [SerializeField] private Button _replayButton;
        [Space(15)]
        [SerializeField] private Color _ourRecordTextColor;
        [SerializeField] private Color _otherUserRecordTextColor;

        public void SetupEmptyRecord() => SetupRecord("", "", new BadgeData(), null, (levelName, data) => { });
        
        public void SetupRecord(string displayName, string levelName, BadgeData badgeData, FriendsLevelRanking.FriendLevelData friendLevelData, Action<string, FriendsLevelRanking.FriendLevelData> replayCallback)
        {
            if (friendLevelData != null)
            {
                _friendsUsernameLabel.text = displayName;
                _friendsUsernameLabel.color = friendLevelData.IsOurRecord ? _ourRecordTextColor : _otherUserRecordTextColor;

                _perfectIcon.gameObject.SetActiveSafe(badgeData.IsPerfect && !badgeData.HasPerfectGoldTime);
                _goldTimeIcon.gameObject.SetActiveSafe(badgeData.HasGoldTime && !badgeData.HasPerfectGoldTime);
                _perfectGoldIcon.gameObject.SetActiveSafe(badgeData.HasPerfectGoldTime);
                    
                _levelTimeLabel.text = friendLevelData.Time.ToString("0.00");

                _replayButton.interactable = true;
                _replayButton.onClick.RemoveAllListeners();
                _replayButton.onClick.AddListener(() => { replayCallback(levelName, friendLevelData); });
            }
            else
            {
                _friendsUsernameLabel.text = "-";
                _levelTimeLabel.text = "[none]";
                _replayButton.interactable = false;

                _perfectIcon.gameObject.SetActiveSafe(false);
                _goldTimeIcon.gameObject.SetActiveSafe(false);
                _perfectGoldIcon.gameObject.SetActiveSafe(false);
            }
        }
    }
}