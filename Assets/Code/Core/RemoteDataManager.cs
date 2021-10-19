using System;
using System.Collections.Generic;
using System.Linq;
using Code.Debugging;
using Code.Level.Player;
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
        public bool IsLoggedIn { get; private set; } = false;
        public string OurPlayFabId { get; private set; } = "";
        public string OurDisplayName { get; private set; } = "";
        public readonly HashSet<string> FriendDisplayNames = new HashSet<string>();

        public void Login(string username, Action<bool> onCompleteCallback)
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                CustomId = username,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetUserAccountInfo = true,
                    GetPlayerProfile = true
                }
                
            }, result =>
            {
                OurPlayFabId = result.PlayFabId;
                UpdateFriendsList();
                
                UserAccountInfo userAccountInfo = result.InfoResultPayload.AccountInfo;
                UserTitleInfo userTitleInfo = userAccountInfo.TitleInfo;
                if (string.IsNullOrEmpty(userTitleInfo.DisplayName) || userTitleInfo.DisplayName.EqualsIgnoreCase(" "))
                {
                    string newUserName = Social.localUser.userName;
                    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                    {
                        DisplayName = newUserName
                    }, nameResult =>
                    {
                        IsLoggedIn = true;
                        OurDisplayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
                        CircumDebug.Log($"Logged in with playfab and updated name to {newUserName}");
                        onCompleteCallback(true);
                    }, error =>
                    {
                        IsLoggedIn = true;
                        CircumDebug.LogError($"Logged in with playfab but couldn't update username : {error.ErrorMessage}");
                        onCompleteCallback(true);
                    });
                }
                else
                {
                    IsLoggedIn = true;
                    OurDisplayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
                    CircumDebug.Log("Logged in with playfab");
                    onCompleteCallback(true);
                }
                
            }, error =>
            {
                CircumDebug.LogError(error.ToString());
                onCompleteCallback(false);
            });
        }
        
        public void LoginWithSocialAPI(Action<bool> onCompleteCallback)
        {
            ILocalUser localUser = Social.localUser;
            localUser.Authenticate(socialAuthenticateSuccess =>
            {
                CircumDebug.Log(socialAuthenticateSuccess
                    ? $"Authentication successful\nUsername: {localUser.userName}\nUser ID: {localUser.id}"
                    : "Authentication failed");

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

        public void UpdateFriendsList(Action<bool> onComplete = null)
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), result =>
            {
                FriendDisplayNames.Clear();
                result.Friends.ApplyFunction(p => FriendDisplayNames.Add(p.TitleDisplayName));
                onComplete?.Invoke(true);
            }, error =>
            {
                CircumDebug.LogError(error.ToString());
                onComplete?.Invoke(false);
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
    }
}