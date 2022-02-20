using System.Collections;
using System.Linq;
using Code.Core;
using Lean.Localization;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.UI
{
    public class Leaderboard : UnityCommonFeatures.UIPanel
    {
        private const float UpdateLeaderboardDelay = 3f;
        
        [SerializeField, Space(30)] private int _getResultsCount;
        [SerializeField] private Transform _scrollRectContent;
        [SerializeField] private GameObject _leaderboardEntryPrefab;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _yourPositionLabel;
        [SerializeField, LeanTranslationName] private string _yourPositionLocalisationTerm;

        private Coroutine _updateLeaderboardCoroutine = null;
        
        protected override void InternalAwake()
        {
            base.InternalAwake();
            RemoteDataManager.Instance.LeaderboardUpdated += UpdateLeaderboard;
        }

        protected override void InternalOnDestroy()
        {
            base.InternalOnDestroy();
            RemoteDataManager.Instance.LeaderboardUpdated -= UpdateLeaderboard;
        }

        public void UpdateLeaderboard()
        {
            this.RestartCoroutine(ref _updateLeaderboardCoroutine, UpdateLeaderboardCoroutine());
        }

        private IEnumerator UpdateLeaderboardCoroutine()
        {
            yield return new WaitForSeconds(UpdateLeaderboardDelay);
            
            _scrollRectContent.DestroyAllChildren();
            
            PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
            {
                StatisticName = PersistentDataKeys.TotalScoresStatistic,
                MaxResultsCount = _getResultsCount
            }, result =>
            {
                foreach (PlayerLeaderboardEntry entryData in result.Leaderboard)
                {
                    GameObject entryObject = Instantiate(_leaderboardEntryPrefab, _scrollRectContent);
                    LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();

                    if (entryData.StatValue == 0)
                    {
                        continue;
                    }
                    
                    leaderboardEntry.SetupLeaderboardEntry(entryData.Position + 1, entryData.DisplayName, entryData.StatValue);
                }
            }, Debug.LogError);
            
            PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = PersistentDataKeys.TotalScoresStatistic,
                PlayFabId = RemoteDataManager.Instance.OurPlayFabId,
                MaxResultsCount = 1
            }, result =>
            {
                string yourPositionMessage = LeanLocalization.GetTranslationText(_yourPositionLocalisationTerm);
                yourPositionMessage = string.Format(yourPositionMessage, $"#{result.Leaderboard[0].Position + 1}");
                _yourPositionLabel.text = yourPositionMessage;
            }, Debug.LogError);
        }
    }
}