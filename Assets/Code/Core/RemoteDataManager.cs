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

        //private readonly Dictionary<string, FriendIdentities> _friendsList = new Dictionary<string, FriendIdentities>();

        public void Login(string username, Action<bool> onCompleteCallback)
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                CustomId = username,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetUserAccountInfo = true
                }
                
            }, result =>
            {
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
                    CircumDebug.Log("Logged in with playfab");
                    onCompleteCallback(true);
                }
                
            }, error =>
            {
                CircumDebug.LogError(error.ToString());
                onCompleteCallback(false);
            });
        }

        // public string GetFriendDisplayName(string playfabId)
        // {
        //     bool foundFriend = _friendsList.TryGetValue(playfabId, out var identities);
        //     return foundFriend ? identities.DisplayName : null;
        // }

        // public void AddNewSocialFriends(IUserProfile[] friendIds)
        // {
        //     // update playfab friends
        //     foreach (IUserProfile friendId in friendIds)
        //     {
        //         string friendCircumUsername = friendId.GetCircumUsername();
        //         if (!_friendsList.ContainsKey(friendCircumUsername))
        //         {
        //             AddFriend(friendCircumUsername);
        //         }
        //     }
        // }
        //
        // private void AddFriend(string displayName)
        // {
        //     PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
        //     {
        //         TitleDisplayName = displayName
        //     }, findPlayerResult =>
        //     {
        //         PlayFabClientAPI.AddFriend(new AddFriendRequest
        //         {
        //             FriendUsername = displayName
        //         }, addFriendResult =>
        //         {
        //             CircumDebug.Log($"Added friend {displayName}");
        //             _friendsList.Add(displayName, new FriendIdentities()
        //             {
        //                 CircumUsername = displayName,
        //                 DisplayName = addFriendResult.Request.
        //             } findPlayerResult.AccountInfo.PlayFabId);
        //         }, error =>
        //         {
        //             CircumDebug.LogError($"Could not add friend {displayName} : {error}");
        //         });
        //     }, error =>
        //     {
        //         CircumDebug.Log($"Can't add player {displayName} since they're not signed up");
        //     });
        // }

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
                        
                        // UpdateFriendsList(updatedFriendsList =>
                        // {
                        //     // if (updatedFriendsList)
                        //     // {
                        //     //     AddNewSocialFriends(localUser.friends);
                        //     // }
                        // });
                    });
                }
                else
                {
                    onCompleteCallback(false);
                }
            });
        }
        
        // public void GetFriendData(string friendUsername, string dataKey, Action<bool, string> onCompleteCallback)
        // {
        //     if (!_friendsList.TryGetValue(friendUsername, out string playfabUsername))
        //     {
        //         onCompleteCallback(false, null);
        //         return;
        //     }
        //     
        //     PlayFabClientAPI.GetUserData(new GetUserDataRequest
        //     {
        //         PlayFabId = playfabUsername, 
        //         Keys = new List<string>()
        //         {
        //             dataKey
        //         }
        //     }, result =>
        //     {
        //         onCompleteCallback(true, result.Data[dataKey].Value);
        //     }, error =>
        //     {
        //         onCompleteCallback(false, null);
        //     });
        // }

        // public void UpdateFriendsList(Action<bool> onCompleteCallback)
        // {
        //     PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest()
        //     {
        //         ProfileConstraints = new PlayerProfileViewConstraints()
        //         {
        //             ShowCreated = true,
        //             ShowLocations = true, 
        //             ShowMemberships = true,
        //             ShowOrigination = true,
        //             ShowStatistics = true, 
        //             ShowTags = true,
        //             ShowAvatarUrl = true,
        //             ShowBannedUntil = true,
        //             ShowCampaignAttributions = true,
        //             ShowDisplayName = true,
        //             ShowExperimentVariants = true,
        //             ShowLastLogin = true,
        //             ShowLinkedAccounts = true,
        //             ShowContactEmailAddresses = true, 
        //             ShowPushNotificationRegistrations = true,
        //             ShowValuesToDate = true,
        //             ShowTotalValueToDateInUsd = true,
        //         }
        //     }, result =>
        //     {
        //         foreach (FriendInfo friendInfo in result.Friends)
        //         {
        //             if (_friendsList.ContainsKey(friendInfo.FriendPlayFabId))
        //             {
        //                 continue;
        //             }
        //             
        //             _friendsList.Add(friendInfo.FriendPlayFabId, new FriendIdentities()
        //             {
        //                 DisplayName = friendInfo.TitleDisplayName,
        //                 CircumUsername = "" 
        //             });
        //         }
        //
        //         onCompleteCallback(true);
        //     }, error =>
        //     {
        //         CircumDebug.LogError($"Could not get friend data : {error}");
        //         onCompleteCallback(false);
        //     });
        // }
        
        public void SetString(string key, string value, Action<bool> onCompleteCallback = null)
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
    }
}