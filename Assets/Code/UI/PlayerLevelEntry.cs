using System;
using Code.Core;
using Code.Level.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Core;

namespace Code.UI
{
    public class PlayerLevelEntry : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset _latinFont;
        [SerializeField] private TMP_FontAsset _chineseFont;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _friendsUsernameLabel;
        [SerializeField] private TextMeshProUGUI _levelTimeLabel;
        [SerializeField] private GameObject _firstPlaceIcon;
        [SerializeField] private Image _perfectIcon;
        [SerializeField] private Image _goldTimeIcon;
        [SerializeField] private Image _perfectGoldIcon;
        [SerializeField] private Button _replayButton;
        [Space(15)]
        [SerializeField] private Color _ourRecordTextColor;
        [SerializeField] private Color _otherUserRecordTextColor;

        public void SetupEmptyRecord() => SetupRecord("", "", new BadgeData(), null, (levelName, data) => { });
        
        public void SetupRecord(string displayName, string levelName, BadgeData badgeData, PlayerLevelData playerLevelData, Action<string, PlayerLevelData> replayCallback)
        {
            if (playerLevelData != null)
            {
                _friendsUsernameLabel.text = displayName;
                _friendsUsernameLabel.font = displayName.HasChineseCharacters() ? _chineseFont : _latinFont;
                _friendsUsernameLabel.color = playerLevelData.IsOurRecord ? _ourRecordTextColor : _otherUserRecordTextColor;

                _perfectIcon.gameObject.SetActiveSafe(badgeData.IsPerfect && !badgeData.HasPerfectGoldTime);
                _goldTimeIcon.gameObject.SetActiveSafe(badgeData.HasGoldTime && !badgeData.HasPerfectGoldTime);
                _perfectGoldIcon.gameObject.SetActiveSafe(badgeData.HasPerfectGoldTime);
                    
                _levelTimeLabel.text = playerLevelData.Time.ToString("0.00");

                _replayButton.interactable = true;
                _replayButton.onClick.RemoveAllListeners();
                _replayButton.onClick.AddListener(() => { replayCallback(levelName, playerLevelData); });

                _firstPlaceIcon.SetActiveSafe(true);
            }
            else
            {
                _friendsUsernameLabel.text = "-";
                _levelTimeLabel.text = "[none]";
                _replayButton.interactable = false;

                _perfectIcon.gameObject.SetActiveSafe(false);
                _goldTimeIcon.gameObject.SetActiveSafe(false);
                _perfectGoldIcon.gameObject.SetActiveSafe(false);
                
                _firstPlaceIcon.SetActiveSafe(false);
            }
        }
    }
}