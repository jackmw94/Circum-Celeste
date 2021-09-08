using System;
using System.Text;
using Code.Debugging;
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
        [SerializeField] private TextMeshProUGUI _goldTimeLabel;
        [Space(15)]
        [SerializeField] private Button _fastestTimeReplayButton;
        [SerializeField] private Button _fastestPerfectTimeReplayButton;

        public void SetupRecordsScreen(float goldTime, LevelRecording fastestTime, LevelRecording fastestPerfectTime, Action<LevelRecording> replayRecording)
        {
            _goldTimeLabel.text = GetTimeString(goldTime);
            SetupRecord(fastestTime, _fastestTimeLabel, _fastestTimeReplayButton, replayRecording);
            SetupRecord(fastestPerfectTime, _fastestPerfectTimeLabel, _fastestPerfectTimeReplayButton, replayRecording);
        }

        private static string GetTimeString(float goldTime)
        {
            int minutes = Mathf.FloorToInt(goldTime / 60f);
            float seconds = goldTime - minutes * 60f;

            StringBuilder sb = new StringBuilder();

            if (minutes > 0)
            {
                sb.Append($"{minutes}m");
            }

            if (Mathf.Abs(seconds - Mathf.Round(seconds)) > float.Epsilon)
            {
                // there is a decimal place
                sb.Append($"{seconds:F1}s");
            }
            else
            {
                // no decimal place
                sb.Append($"{Mathf.RoundToInt(seconds)}s");
            }
            
            // 130.5 -> 2m10.5s
            
            return sb.ToString(); 
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