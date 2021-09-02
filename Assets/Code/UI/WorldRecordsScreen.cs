using System;
using Code.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class WorldRecordsScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _fastestTimeLabel;
        [SerializeField] private TextMeshProUGUI _fastestPerfectTimeLabel;
        [Space(15)]
        [SerializeField] private Button _fastestTimeReplayButton;
        [SerializeField] private Button _fastestPerfectTimeReplayButton;

        public void SetupRecordsScreen(LevelRecording fastestTime, LevelRecording fastestPerfectTime, Action<LevelRecording> replayRecording)
        {
            SetupRecord(fastestTime, _fastestTimeLabel, _fastestTimeReplayButton, replayRecording);
            SetupRecord(fastestPerfectTime, _fastestPerfectTimeLabel, _fastestPerfectTimeReplayButton, replayRecording);
        }

        private static void SetupRecord(LevelRecording levelRecording, TextMeshProUGUI timeLabel, Button replayButton, Action<LevelRecording> replayCallback)
        {
            if (levelRecording != null)
            {
                timeLabel.text = levelRecording.RecordingData.LevelTime.ToString("0.00");
                
                replayButton.interactable = true;
                replayButton.onClick.RemoveAllListeners();
                replayButton.onClick.AddListener(() =>
                {
                    replayCallback(levelRecording);
                });
            }
            else
            {
                timeLabel.text = "[none]";
                replayButton.interactable = false;
            }
        }
    }
}