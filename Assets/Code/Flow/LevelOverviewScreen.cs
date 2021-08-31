using System;
using Code.Debugging;
using Code.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Flow
{
    public class LevelOverviewScreen : MonoBehaviour
    {
        private const string LevelDurationTokenName = "LevelDuration";
        
        [SerializeField] private TextMeshProUGUI _levelNumber;
        [SerializeField] private TextMeshProUGUI _levelName;
        [SerializeField] private TextMeshProUGUI _levelTag;
        [Space(15)]
        [SerializeField] private GameObject _perfectIcon;
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

        public void SetupLevelOverview(LevelLayout levelLayout, bool isPerfect, Action playLevelCallback)
        {
            int levelNumber = levelLayout.LevelContext.LevelNumber;
            
            _levelNumber.text = $"{levelNumber}";
            _levelNumber.enabled = levelNumber > 0;
            
            _levelName.text = levelLayout.name;

            SetTagString(levelLayout);

            _perfectIcon.SetActive(isPerfect);
            _playLevelCallback = playLevelCallback;
        }

        private void SetTagString(LevelLayout levelLayout)
        {
            string escapeTimeString = Mathf.RoundToInt(levelLayout.EscapeTimer).ToString();
            Lean.Localization.LeanLocalization.SetToken(LevelDurationTokenName, escapeTimeString);
            
            string tagText = Lean.Localization.LeanLocalization.GetTranslationText(levelLayout.TagLineLocalisationTerm, "");
            CircumDebug.Assert(string.IsNullOrEmpty(levelLayout.TagLineLocalisationTerm) == string.IsNullOrEmpty(tagText), $"Error getting localisation term {levelLayout.TagLineLocalisationTerm} (localised='{tagText}')");
            
            _levelTag.text = tagText;
        }
        
        private void PlayButtonClicked()
        {
            _playLevelCallback();
        }
    }
}