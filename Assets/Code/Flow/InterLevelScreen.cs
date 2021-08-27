using System.Collections;
using Code.Level;
using Code.Level.Player;
using Code.UI;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Flow
{
    public class InterLevelScreen : MonoBehaviour
    {
        [SerializeField] private float _showHideDuration = 0.5f;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ScrollingItemPicker _scrollingItemPicker;
        [Space(15)]
        [SerializeField] private LevelOverviewScreen _levelOverviewScreen;
        [SerializeField] private WorldRecordsScreen _worldRecordsScreen;
        [SerializeField] private RunOverviewScreen _runOverviewScreen;
        [Space(15)]
        [SerializeField] private PlayerStatsManager _playerStatsManager;
        [SerializeField] private LevelProvider _levelProvider;
        [SerializeField] private LevelManager _levelManager;

        public void SetupInterLevelScreen()
        {
            LevelLayout levelLayout = _levelProvider.GetCurrentLevel();
            
            LevelRecording levelRecording = _playerStatsManager.PlayerStats.GetRecordingForLevelAtIndex(levelLayout.LevelContext.LevelIndex);
            _worldRecordsScreen.SetupRecordsScreen(levelRecording, null, ReplayLevel);
            
            _levelOverviewScreen.SetupLevelOverview(levelLayout, PlayLevel);
            
            _scrollingItemPicker.SetToItemAtIndex(_scrollingItemPicker.NumberOfItems - 1);
        }

        private void PlayLevel()
        {
            _levelManager.CreateCurrentLevel();
        }

        private void ReplayLevel(LevelRecording levelRecording)
        {
            _levelManager.CreateCurrentLevel(levelRecording);
        }
        
        public IEnumerator ShowHideScreen(bool show)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = false;
            
            float targetAlpha = show ? 1f : 0f;
            yield return Utilities.LerpOverTime(_canvasGroup.alpha, targetAlpha, _showHideDuration, f =>
            {
                _canvasGroup.alpha = f;
            });
            
            _canvasGroup.blocksRaycasts = show;
            _canvasGroup.interactable = show;
        }
    }
}