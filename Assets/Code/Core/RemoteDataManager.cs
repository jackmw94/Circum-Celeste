using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Debugging;
using Code.Flow;
using Code.Level.Player;
using Code.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
    public class RemoteDataManager : SingletonMonoBehaviour<RemoteDataManager>
    {
        private const float EditorFriendRefreshDelay = 10f;
        private const float PlatformFriendRefreshDelay = 45f;
        
        public readonly HashSet<string> FriendDisplayNames = new HashSet<string>();

        private Coroutine _checkNewFriendRequest = null;
        
        public bool IsLoggedIn { get; private set; } = false;
        public string OurPlayFabId { get; private set; } = "";
        public string OurDisplayName { get; private set; } = "";
        public Action LeaderboardUpdated { get; set; } = () => { };

        private float CheckFriendsRefreshDelay => Application.isEditor ? EditorFriendRefreshDelay : PlatformFriendRefreshDelay;


        private void Awake()
        {
#if UNITY_ANDROID
#if CIRCUM_LOGGING
            GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = true;
#endif
            
#if !UNITY_EDITOR
            GooglePlayGames.BasicApi.PlayGamesClientConfiguration config = new 
            GooglePlayGames.BasicApi.PlayGamesClientConfiguration.Builder().Build();
            GooglePlayGames.PlayGamesPlatform.InitializeInstance(config);
            
            GooglePlayGames.PlayGamesPlatform platform = GooglePlayGames.PlayGamesPlatform.Activate();
            CircumDebug.Log($"Called activate on google play games. {platform.localUser}");
#endif
#endif
        }

        public void Login(string username, Action<bool> onCompleteCallback)
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                CustomId = username,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetUserAccountInfo = true,
                    GetPlayerProfile = true,
                    GetUserData = true,
                    UserDataKeys = new List<string> {PersistentDataKeys.PlayerHighscore}
                }
                
            }, result =>
            {
                OurPlayFabId = result.PlayFabId;
                
                UserAccountInfo userAccountInfo = result.InfoResultPayload.AccountInfo;
                UserTitleInfo userTitleInfo = userAccountInfo.TitleInfo;
                if (string.IsNullOrEmpty(userTitleInfo.DisplayName) || userTitleInfo.DisplayName.EqualsIgnoreCase(" "))
                {
                    // new user
                    
                    string newUserName = Social.localUser.userName;
                    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                    {
                        DisplayName = newUserName
                    }, nameResult =>
                    {
                        IsLoggedIn = true;
                        _checkNewFriendRequest = StartCoroutine(CheckNewFriendCoroutine());
                        OurDisplayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
                        CircumDebug.Log($"Logged in with playfab and updated name to {newUserName}");
                        onCompleteCallback(true);
                    }, error =>
                    {
                        IsLoggedIn = true;
                        _checkNewFriendRequest = StartCoroutine(CheckNewFriendCoroutine());
                        CircumDebug.LogError($"Logged in with playfab but couldn't update username : {error.ErrorMessage}");
                        onCompleteCallback(true);
                    });
                }
                else
                {
                    // existing user
                    
                    IsLoggedIn = true;
                    _checkNewFriendRequest = StartCoroutine(CheckNewFriendCoroutine());
                    OurDisplayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
                    CircumDebug.Log("Logged in with playfab");
                    onCompleteCallback(true);

                    if (!result.InfoResultPayload.UserData.ContainsKey(PersistentDataKeys.PlayerHighscore))
                    {
                        PlayerScoreHelper.PlayerScore playerScore = PersistentDataManager.Instance.UpdateUserScore();
                        CircumDebug.Log($"Player score : \n{JsonUtility.ToJson(playerScore)}");
                        UpdateUserScore(playerScore);
                    }
                }
                
            }, error =>
            {
                CircumDebug.LogError(error.ToString());
                onCompleteCallback(false);
            });
        }

        public void UpdateUserScore(PlayerScoreHelper.PlayerScore playerScore)
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = PersistentDataKeys.TotalScoresStatistic,
                        Value = playerScore.TotalScore
                    }
                }
            }, result =>
            {
                CircumDebug.Log($"Updated player total score to {playerScore.TotalScore}");
                LeaderboardUpdated();
            }, error =>
            {
                CircumDebug.LogError(error.ErrorMessage);
            });
        }
        
        public void LoginWithSocialAPI(Action<bool> onCompleteCallback)
        {
            if (Social.Active == null)
            {
                CircumDebug.Log("There was no active social platform! Cannot log user in");
                onCompleteCallback(false);
                return;
            }
            
            ILocalUser localUser = Social.localUser;
            CircumDebug.Log($"Logging in with social API. User: {localUser.userName} ({localUser.id})");

            localUser.Authenticate((socialAuthenticateSuccess, messages) =>
            {
                CircumDebug.Log(socialAuthenticateSuccess
                    ? $"Authentication successful: {messages}"
                    : $"Authentication failed: {messages}");

                if (socialAuthenticateSuccess)
                {
                    Login(localUser.GetCircumUsername(), (remoteLoginSuccess) =>
                    {
                        onCompleteCallback(true);
                    });
                }
                else
                {
                    onCompleteCallback(false);
                }
            });
        }

        public void UpdateFriendsList(Action<bool, int> onComplete = null)
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), result =>
            {
                CircumDebug.Log($"Updated friends list. Had {FriendDisplayNames.Count} cached friends, found {result.Friends.Count} friends, known count = {PersistentDataManager.Instance.PlayerStats.KnownFriendsCount}");
                
                FriendDisplayNames.Clear();
                result.Friends.ApplyFunction(p => FriendDisplayNames.Add(p.TitleDisplayName));

                PersistentDataManager persistentDataManager = PersistentDataManager.Instance;
                PlayerStats playerStats = persistentDataManager.PlayerStats;
                int changeInFriendsCount = playerStats.KnownFriendsCount == -1 ? 0 : FriendDisplayNames.Count - playerStats.KnownFriendsCount;

                if (FriendDisplayNames.Count != playerStats.KnownFriendsCount)
                {
                    playerStats.SetKnownFriendsCount(FriendDisplayNames.Count);
                    PlayerStats.Save(playerStats);
                }
                
                onComplete?.Invoke(true, changeInFriendsCount);
            }, error =>
            {
                CircumDebug.LogError(error.ToString());
                onComplete?.Invoke(false, 0);
            });
        }
        
        public void SetUserData(string key, string value, Action<bool> onCompleteCallback = null)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {key, value}
                },
                Permission = UserDataPermission.Public
            }, result =>
            {
                onCompleteCallback?.Invoke(true);
            }, error =>
            {
                CircumDebug.LogError($"Could not update key {key} with value {value} : {error}");
                onCompleteCallback?.Invoke(false);
            });
        }

        public void ResetStats()
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "resetAllUserData"
            }, result =>
            {
                CircumDebug.Log($"Reset all remote user data\n{result.Logs.Select(p => p.Message).JoinToString("\n")}");
            }, error =>
            {
                CircumDebug.LogError($"Could not reset remote user data {error.GenerateErrorReport()}");
            });
        }

        private IEnumerator CheckNewFriendCoroutine()
        {
            yield return new WaitForSeconds(5f);
            
            while (true)
            {
                SendCheckNewFriendRequest();
                yield return new WaitForSeconds(CheckFriendsRefreshDelay);
            }
        }

        private void SendCheckNewFriendRequest()
        {
            UpdateFriendsList((success, friendsCountDifference) =>
            {
                if (success && friendsCountDifference > 0 && ShouldNotifyUserOfNewFriends())
                {
                    PersistentDataManager.Instance.PlayerFirsts.ShowNewFriendPopupIfFirst();
                    Settings.Instance.ShowNewFriendsNotification(friendsCountDifference);
                }
            });
        }

        private bool ShouldNotifyUserOfNewFriends()
        {
            return !Settings.Instance.SettingsAreShowing && !AddFriendsScreen.Instance.IsShowing;
        }
    }
}