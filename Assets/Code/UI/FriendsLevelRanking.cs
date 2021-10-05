using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Core;
using Code.Level;
using Code.Level.Player;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.UI
{
    public class FriendsLevelRanking : MonoBehaviour
    {
        [Serializable]
        public class FriendLevelsData
        {
            [SerializeField] private FriendLevelData[] _data;
            public FriendLevelData[] Data => _data;
        }

        [Serializable]
        public class FriendLevelData
        {
            [SerializeField] private string _username;
            [SerializeField] private float _time;
            [SerializeField] private bool _isPerfect;

            public string Username => _username;
            public float Time => _time;
            public bool IsPerfect => _isPerfect;
        }

        [SerializeField] private FriendsLevelEntry _firstPlace;
        [SerializeField] private FriendsLevelEntry _secondPlace;
        [SerializeField] private FriendsLevelEntry _thirdPlace;

        private Action<LevelRecording> _replayRecording = null;
        private Coroutine _updateLevelCoroutine = null;

        public void SetupRecordsScreen(string levelName, float goldTime, Action<LevelRecording> replayRecording)
        {
            _replayRecording = replayRecording;

            _firstPlace.SetupEmptyRecord();
            _secondPlace.SetupEmptyRecord();
            _thirdPlace.SetupEmptyRecord();

            UpdateLevel(levelName, goldTime);
        }

        private void UpdateLevel(string levelName, float goldTime)
        {
            if (_updateLevelCoroutine != null)
            {
                StopCoroutine(_updateLevelCoroutine);
            }

            _updateLevelCoroutine = StartCoroutine(UpdateLevelCoroutine(levelName, goldTime));
        }

        private IEnumerator UpdateLevelCoroutine(string levelName, float goldTime)
        {
            yield return new WaitUntil(() => RemoteDataManager.Instance.IsLoggedIn);
            
            string statsKey = PersistentDataKeys.LevelMetaStats(levelName);
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName = "getBestLevels",
                FunctionParameter = new { levelDataKey = statsKey }
            }, scriptResult =>
            {
                Debug.Log($"Returned from get best level\n{scriptResult.Logs.Select(p => p.Message).JoinToString("\n")}");
                
                if (scriptResult.Error != null)
                {
                    Debug.LogError($"{scriptResult.Error.Error} : {scriptResult.Error.Message}\n{scriptResult.Error.StackTrace}");
                }
                
                string serialized = scriptResult.FunctionResult.ToString();
                FriendLevelsData deserialized = JsonUtility.FromJson<FriendLevelsData>(serialized);
                
                PopulatePlaces(deserialized, goldTime);
                
            }, error =>
            {
                Debug.LogError(error.ErrorMessage);
            });
        }

        private void PopulatePlaces(FriendLevelsData friendLevelsData, float goldTime)
        {
            if (friendLevelsData.Data.Length > 0)
            {
                FriendLevelData friendLevelData = friendLevelsData.Data[0];
                BadgeData badgeData = GetBadgeData(friendLevelData, goldTime);
                _firstPlace.SetupRecord(friendLevelData.Username, badgeData, friendLevelData, RequestReplay);
            }
            
            if (friendLevelsData.Data.Length > 1)
            {
                FriendLevelData friendLevelData = friendLevelsData.Data[1];
                BadgeData badgeData = GetBadgeData(friendLevelData, goldTime);
                _secondPlace.SetupRecord(friendLevelData.Username, badgeData, friendLevelData, RequestReplay);
            }
            
            if (friendLevelsData.Data.Length > 2)
            {
                FriendLevelData friendLevelData = friendLevelsData.Data[2];
                BadgeData badgeData = GetBadgeData(friendLevelData, goldTime);
                _thirdPlace.SetupRecord(friendLevelData.Username, badgeData, friendLevelData, RequestReplay);
            }
        }

        private BadgeData GetBadgeData(FriendLevelData friendLevelData, float goldTime)
        {
            bool hasGoldTime = friendLevelData.Time <= goldTime;
            bool isPerfect = friendLevelData.IsPerfect;
            BadgeData badgeData = new BadgeData()
            {
                IsPerfect = isPerfect,
                HasGoldTime = hasGoldTime,
                HasPerfectGoldTime = isPerfect && hasGoldTime
            };

            return badgeData;
        }

        private void RequestReplay(FriendLevelData friendLevelData)
        {
            
        }
    }
}