using System;
using System.Collections;
using System.Collections.Generic;
using Code.Core;
using Code.Debugging;
using Code.Level.Player;
using Lean.Localization;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.Singletons;

namespace Code.UI
{
    public class AddFriendsScreen : SingletonMonoBehaviour<AddFriendsScreen>
    {
        [SerializeField] private TextMeshProUGUI _uniqueIdLabel;
        [SerializeField, LeanTranslationName] private string _notLoggedInLocalisationTerm;
        [Space(15)]
        [SerializeField] private TMP_InputField _friendIdInput;
        [SerializeField] private Button _addFriendButton;
        [SerializeField] private PlayerLevelRankingPanel _friendsLevelRanking;
        [Space(15)]
        [SerializeField] private Transform _friendsListRoot;
        [SerializeField] private GameObject _friendEntryPrefab;
        [SerializeField] private Button _refreshFriendsButton;
        [SerializeField] private GameObject _noFriendsIndicator;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _messageLabel;
        [SerializeField] private Color _successColour;
        [SerializeField] private Color _errorColour;
        [SerializeField] private float _showMessageForDuration = 5f;
        [SerializeField, LeanTranslationName] private string _addedFriendLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _idNotFoundLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _alreadyFriendsLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _couldNotAddErrorLocalisationTerm;
        [SerializeField, LeanTranslationName] private string _cantAddYourselfAsFriendLocalisationTerm;

        private Coroutine _showMessageCoroutine = null;

        public bool IsShowing { get; private set; } = false;
        
        private void Awake()
        {
            _addFriendButton.onClick.AddListener(AddFriendButtonListener);
            _refreshFriendsButton.onClick.AddListener(RefreshFriendsButtonListener);
        }

        private void OnDestroy()
        {
            _addFriendButton.onClick.RemoveListener(AddFriendButtonListener);
            _refreshFriendsButton.onClick.RemoveListener(RefreshFriendsButtonListener);
        }

        private void OnEnable()
        {
            _messageLabel.text = "";
            _uniqueIdLabel.text = RemoteDataManager.Instance.IsLoggedIn ? 
                RemoteDataManager.Instance.OurPlayFabId : 
                LeanLocalization.GetTranslationText(_notLoggedInLocalisationTerm, "[user not logged in]");
            RefreshFriendsListUI();

            IsShowing = true;
        }

        private void OnDisable()
        {
            IsShowing = false;
        }

        private void RefreshFriendsButtonListener()
        {
            _friendsListRoot.DestroyAllChildren();
            RemoteDataManager.Instance.UpdateFriendsList((_1, _2) =>
            {
                RefreshFriendsListUI();
            });
        }

        private void RefreshFriendsListUI()
        {
            _friendsListRoot.DestroyAllChildren();
            HashSet<string> friendDisplayNames = RemoteDataManager.Instance.FriendDisplayNames;
            friendDisplayNames.ApplyFunction(friendName =>
            {
                GameObject friendEntryInstance = Instantiate(_friendEntryPrefab, _friendsListRoot);
                TextMeshProUGUI friendNameLabel = friendEntryInstance.GetComponentInChildren<TextMeshProUGUI>();
                friendNameLabel.text = friendName;
            });
            _noFriendsIndicator.SetActiveSafe(friendDisplayNames.Count == 0);
        }

        private void AddFriendButtonListener()
        {
            string friendPlayFabId = _friendIdInput.text;
            if (friendPlayFabId == RemoteDataManager.Instance.OurPlayFabId)
            {
                ShowMessage(_cantAddYourselfAsFriendLocalisationTerm, true);
                return;
            }
            
            _addFriendButton.interactable = false;
            PlayFabClientAPI.AddFriend(new AddFriendRequest
            {
                FriendPlayFabId = friendPlayFabId,
            }, result =>
            {
                _addFriendButton.interactable = true;
                ShowMessage(_addedFriendLocalisationTerm, false);
                CircumDebug.Log(result.ToString());
                
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "requiteFriendship",
                    FunctionParameter = new { 
                        ourPlayFabId = RemoteDataManager.Instance.OurPlayFabId,
                        friendPlayFabId = friendPlayFabId
                    }
                }, scriptResult =>
                {
                    CircumDebug.Log(result.ToString());
                }, error =>
                {
                    CircumDebug.LogError(error.ToString());
                });
                
                _friendsLevelRanking.RefreshScreen();
                RemoteDataManager.Instance.UpdateFriendsList((success, _) =>
                {
                    if (success)
                    {
                        RefreshFriendsListUI();
                    }
                });
            }, error =>
            {
                _addFriendButton.interactable = true;
                
                switch (error.Error)
                {
                    case PlayFabErrorCode.AccountNotFound:
                    case PlayFabErrorCode.AccountBanned:
                    case PlayFabErrorCode.AccountDeleted:
                        ShowMessage(_idNotFoundLocalisationTerm, true);
                        break;
                    case PlayFabErrorCode.UsersAlreadyFriends:
                        ShowMessage(_alreadyFriendsLocalisationTerm, true);
                        break;
                    default:
                        ShowMessage(_couldNotAddErrorLocalisationTerm, true);
                        break;
                }
                
                CircumDebug.Log(error.Error.ToString());
            });
        }

        private void ShowMessage(string localisationTerm, bool isError)
        {
            if (_showMessageCoroutine != null)
            {
                StopCoroutine(_showMessageCoroutine);
            }

            _showMessageCoroutine = StartCoroutine(ShowMessageCoroutine(localisationTerm, isError));
        }

        private IEnumerator ShowMessageCoroutine(string localisationTerm, bool isError)
        {
            string message = LeanLocalization.GetTranslationText(localisationTerm);
            _messageLabel.text = message;
            _messageLabel.color = isError ? _errorColour : _successColour;
            yield return new WaitForSeconds(_showMessageForDuration);
            _messageLabel.text = "";
        }
    }
}