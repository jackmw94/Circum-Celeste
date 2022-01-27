using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Level;
using Code.Level.Player;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.UI
{
    public abstract class PlayerLevelRankingPanel : MonoBehaviour
    {
        [SerializeField] private UIPanel _loadingPanel;
        [SerializeField] private UIPanel _noFriendsPanel;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private PlayerLevelEntry _firstPlace;
        [SerializeField] private PlayerLevelEntry _secondPlace;
        [SerializeField] private PlayerLevelEntry _thirdPlace;
        [SerializeField] private FriendsScreenSelectable _panelSelectable;

        protected string _currentLevelName;
        private float _currentLevelGoldTime;
        
        private Action<LevelRecording> _replayRecording = null;
        private Coroutine _updateLevelCoroutine = null;

        public void RefreshScreen() => SetupRecordsScreen(_currentLevelName, _currentLevelGoldTime, _replayRecording);

        protected abstract ExecuteCloudScriptRequest GetCloudScriptRequest();

        public void SetupRecordsScreen(string levelName, float goldTime, Action<LevelRecording> replayRecording)
        {
            _currentLevelName = levelName;
            _currentLevelGoldTime = goldTime;
            
            _replayRecording = replayRecording;
            
            _loadingPanel.ShowHideLoading(true, true);
            _noFriendsPanel.ShowHideLoading(false, true);

            _firstPlace.SetupEmptyRecord();
            _secondPlace.SetupEmptyRecord();
            _thirdPlace.SetupEmptyRecord();

            UpdateLevel();
        }

        private void UpdateLevel()
        {
            if (_updateLevelCoroutine != null)
            {
                StopCoroutine(_updateLevelCoroutine);
            }

            _updateLevelCoroutine = StartCoroutine(UpdateLevelCoroutine());
        }

        private IEnumerator UpdateLevelCoroutine()
        {
            yield return new WaitUntil(() => _panelSelectable.IsSelected);
            
            RemoteDataManager remoteDataManager = RemoteDataManager.Instance;
            yield return new WaitUntil(() => remoteDataManager.IsLoggedIn);
            
            PlayFabClientAPI.ExecuteCloudScript(GetCloudScriptRequest(), scriptResult =>
            {
                Debug.Log($"Returned from get best level\n{scriptResult.Logs.Select(p => p.Message).JoinToString("\n")}");
                
                if (scriptResult.Error != null)
                {
                    Debug.LogError($"{scriptResult.Error.Error} : {scriptResult.Error.Message}\n{scriptResult.Error.StackTrace}");
                }
                
                string serialized = scriptResult.FunctionResult.ToString();
                PlayerLevelsData deserialized = JsonUtility.FromJson<PlayerLevelsData>(serialized);
                
                _loadingPanel.ShowHideLoading(false, false);
                _noFriendsPanel.ShowHideLoading(deserialized.TotalNumberOfFriends == 0, false);
                
                PopulatePlaces(_currentLevelName, deserialized, _currentLevelGoldTime);
                
            }, error =>
            {
                Debug.LogError(error.ErrorMessage);
            });
        }

        private void PopulatePlaces(string levelName, PlayerLevelsData playerLevelsData, float goldTime)
        {
            if (playerLevelsData.Data.Length > 0)
            {
                PlayerLevelData playerLevelData = playerLevelsData.Data[0];
                BadgeData badgeData = GetBadgeData(playerLevelData, goldTime);
                _firstPlace.SetupRecord(playerLevelData.Username, levelName, badgeData, playerLevelData, RequestReplay);
            }
            
            if (playerLevelsData.Data.Length > 1)
            {
                PlayerLevelData playerLevelData = playerLevelsData.Data[1];
                BadgeData badgeData = GetBadgeData(playerLevelData, goldTime);
                _secondPlace.SetupRecord(playerLevelData.Username, levelName, badgeData, playerLevelData, RequestReplay);
            }
            
            if (playerLevelsData.Data.Length > 2)
            {
                PlayerLevelData playerLevelData = playerLevelsData.Data[2];
                BadgeData badgeData = GetBadgeData(playerLevelData, goldTime);
                _thirdPlace.SetupRecord(playerLevelData.Username, levelName, badgeData, playerLevelData, RequestReplay);
            }
        }

        private BadgeData GetBadgeData(PlayerLevelData playerLevelData, float goldTime)
        {
            bool hasGoldTime = playerLevelData.Time <= goldTime;
            bool isPerfect = playerLevelData.IsPerfect;
            BadgeData badgeData = new BadgeData()
            {
                IsPerfect = isPerfect,
                HasGoldTime = hasGoldTime,
                HasPerfectGoldTime = isPerfect && hasGoldTime
            };

            return badgeData;
        }

        private void RequestReplay(string levelName, PlayerLevelData playerLevelData)
        {
            // show overlay to prevent further interaction

            string levelKey = PersistentDataKeys.LevelRecording(levelName);
            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = playerLevelData.PlayfabId,
                Keys = new List<string>
                {
                    levelKey
                }
            }, result =>
            {
                Debug.Log($"GOT SOME STUFF : " + result.Data[levelKey].Value.Length);
                string compressedRecording = result.Data[levelKey].Value;
                string decompressedRecording = compressedRecording.Decompress();
                LevelRecording levelRecording = JsonUtility.FromJson<LevelRecording>(decompressedRecording);
                _levelManager.CreateCurrentLevel(levelRecording);
            }, error =>
            {
                Debug.LogError($"Could not get replay : {error.ErrorMessage}");
            });
        }
    }
}