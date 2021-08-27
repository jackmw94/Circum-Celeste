using System;
using Code.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Flow
{
    public class LevelOverviewScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelNumber;
        [SerializeField] private TextMeshProUGUI _levelName;
        [SerializeField] private TextMeshProUGUI _levelTag;
        [Space(15)]
        [SerializeField] private Button _playButton;

        private Action _playLevelCallback = null;

        private void Awake()
        {
            _playButton.onClick.AddListener(PlayButtonClicked);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(PlayButtonClicked);
        }

        public void SetupLevelOverview(LevelLayout levelLayout, Action playLevelCallback)
        {
            int levelNumber = levelLayout.LevelContext.LevelNumber;
            
            _levelNumber.text = $"{levelNumber}";
            _levelNumber.enabled = levelNumber > 0;
            
            _levelName.text = levelLayout.name;
            _levelTag.text = levelLayout.TagLine;

            _playLevelCallback = playLevelCallback;
        }

        private void PlayButtonClicked()
        {
            _playLevelCallback();
        }
    }
}