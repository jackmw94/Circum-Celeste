using System.Collections;
using Code.Core;
using Lean.Localization;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityExtras.Core;

namespace Code.UI
{
    public class Leaderboard : UnityCommonFeatures.UIPanel
    {
        private const float UpdateLeaderboardDelay = 3f;
        
        [SerializeField, Space(30)] private int _getResultsCount;
        [SerializeField] private Transform _scrollRectContent;
        [SerializeField] private GameObject _leaderboardEntryPrefab;
        [Space(15)]
        [SerializeField] private GameObject _content;
        [SerializeField] private GameObject _error;
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
            _scrollRectContent.DestroyAllChildren();

            UpdateYourPosition("-");

            if (!RemoteDataManager.Instance.IsLoggedIn)
            {
                ShowContentOrError(contentOn: false);
                yield break;
            }

            yield return new WaitForSeconds(UpdateLeaderboardDelay);
            
            PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
            {
                StatisticName = PersistentDataKeys.TotalScoresStatistic,
                MaxResultsCount = _getResultsCount
            }, result =>
            {
                foreach (PlayerLeaderboardEntry entryData in result.Leaderboard)
                {
                    if (entryData.StatValue == 0)
                    {
                        continue;
                    }
                    
                    GameObject entryObject = Instantiate(_leaderboardEntryPrefab, _scrollRectContent);
                    LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();
                    
                    leaderboardEntry.SetupLeaderboardEntry(entryData.Position + 1, entryData.DisplayName, entryData.StatValue);
                }
                ShowContentOrError(contentOn: true);
            }, error =>
            {
                Debug.LogError(error);
                ShowContentOrError(contentOn: false);
            });
            
            PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = PersistentDataKeys.TotalScoresStatistic,
                PlayFabId = RemoteDataManager.Instance.OurPlayFabId,
                MaxResultsCount = 1
            }, result =>
            {
                int position = result.Leaderboard[0].Position + 1;
                UpdateYourPosition($"#{position}");
            }, Debug.LogError);
        }

        private void ShowContentOrError(bool contentOn)
        {
            _content.SetActive(contentOn);
            _error.SetActive(!contentOn);
        }

        private void UpdateYourPosition(string position)
        {
            string yourPositionMessage = LeanLocalization.GetTranslationText(_yourPositionLocalisationTerm);
            yourPositionMessage = string.Format(yourPositionMessage, position);
            _yourPositionLabel.text = yourPositionMessage;
        }
    }
}