using System;
using System.Text;
using Code.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class FriendsLevelRanking : MonoBehaviour
    {
        [Serializable]
        private class FriendsLevelEntry
        {
            public TextMeshProUGUI _friendsUsernameLabel;
            public TextMeshProUGUI _levelTimeLabel;
            public Button _replayButton;
            
            public void SetupRecord(string username, LevelRecording levelRecording, Action<LevelRecording> replayCallback)
            {
                if (levelRecording != null)
                {
                    _friendsUsernameLabel.text = username;
                    _levelTimeLabel.text = levelRecording.RecordingData.LevelTime.ToString("0.00");
                
                    _replayButton.interactable = true;
                    _replayButton.onClick.RemoveAllListeners();
                    _replayButton.onClick.AddListener(() =>
                    {
                        replayCallback(levelRecording);
                    });
                }
                else
                {
                    _levelTimeLabel.text = "[none]";
                    _replayButton.interactable = false;
                }
            }
        }
        
        [SerializeField] private FriendsLevelEntry _firstPlace;
        [SerializeField] private FriendsLevelEntry _secondPlace;
        [SerializeField] private FriendsLevelEntry _thirdPlace;

        public void SetupRecordsScreen(float goldTime, Action<LevelRecording> replayRecording)
        {
            //_firstPlace.SetupRecord();
        }
    }
}