using System;
using System.Collections.Generic;
using System.Linq;
using Code.Debugging;
using Code.Level.Player;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
    public class RemoteDataManager : SingletonMonoBehaviour<RemoteDataManager>
    {
        public bool IsLoggedIn { get; private set; } = false;

        private readonly Dictionary<string, string> _friendsList = new Dictionary<string, string>();

        public void Login(string username, Action<bool> onCompleteCallback)
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
            {
                CustomId = username,
                CreateAccount = true
            }, result =>
            {
                IsLoggedIn = true;
                CircumDebug.Log("Logged in with playfab");
                onCompleteCallback(true);
            }, error =>
            {
                CircumDebug.LogError(error.ToString());
                onCompleteCallback(false);
            });
        }

        public void AddNewSocialFriends(IUserProfile[] friendIds)
        {
            // update playfab friends
            foreach (IUserProfile friendId in friendIds)
            {
                string friendCircumUsername = friendId.GetCircumUsername();
                if (!_friendsList.ContainsKey(friendCircumUsername))
                {
                    AddFriend(friendCircumUsername);
                }
            }
        }

        private void AddFriend(string circumUsername)
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
            {
                Username = circumUsername
            }, findPlayerResult =>
            {
                PlayFabClientAPI.AddFriend(new AddFriendRequest
                {
                    FriendUsername = circumUsername
                }, addFriendResult =>
                {
                    CircumDebug.Log($"Added friend {circumUsername}");
                    _friendsList.Add(circumUsername, findPlayerResult.AccountInfo.PlayFabId);
                }, error =>
                {
                    CircumDebug.LogError($"Could not add friend {circumUsername} : {error}");
                });
            }, error =>
            {
                CircumDebug.Log($"Can't add player {circumUsername} since they're not signed up");
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
                        
                        UpdateFriendsList(updatedFriendsList =>
                        {
                            if (updatedFriendsList)
                            {
                                AddNewSocialFriends(localUser.friends);
                            }
                        });
                    });
                }
                else
                {
                    onCompleteCallback(false);
                }
            });
        }
        
        public void GetFriendData(string friendUsername, string dataKey, Action<bool, string> onCompleteCallback)
        {
            if (!_friendsList.TryGetValue(friendUsername, out string playfabUsername))
            {
                onCompleteCallback(false, null);
                return;
            }
            
            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = playfabUsername, 
                Keys = new List<string>()
                {
                    dataKey
                }
            }, result =>
            {
                onCompleteCallback(true, result.Data[dataKey].Value);
            }, error =>
            {
                onCompleteCallback(false, null);
            });
        }

        public void UpdateFriendsList(Action<bool> onCompleteCallback)
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), result =>
            {
                foreach (FriendInfo friendInfo in result.Friends)
                {
                    if (_friendsList.ContainsKey(friendInfo.Username))
                    {
                        continue;
                    }
                    
                    _friendsList.Add(friendInfo.Username, friendInfo.FriendPlayFabId);
                }

                onCompleteCallback(true);
            }, error =>
            {
                CircumDebug.LogError($"Could not get friend data : {error}");
                onCompleteCallback(false);
            });
        }
        
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