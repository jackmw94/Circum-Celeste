using System.Collections;
using Code.Core;
using Code.Debugging;
using Code.Level.Player;
using Lean.Localization;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtras.Code.Core;

namespace Code.UI
{
    public class AddFriendsScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _uniqueIdLabel;
        [SerializeField, LeanTranslationName] private string _notLoggedInLocalisationTerm;
        [Space(15)]
        [SerializeField] private TMP_InputField _friendIdInput;
        [SerializeField] private Button _addFriendButton;
        [SerializeField] private FriendsLevelRanking _friendsLevelScreen;
        [Space(15)]
        [SerializeField] private Transform _friendsListRoot;
        [SerializeField] private GameObject _friendEntryPrefab;
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
        
        private void Awake()
        {
            _addFriendButton.onClick.AddListener(AddFriendButtonListener);
        }

        private void OnDestroy()
        {
            _addFriendButton.onClick.RemoveListener(AddFriendButtonListener);
        }

        private void OnEnable()
        {
            _messageLabel.text = "";
            _uniqueIdLabel.text = RemoteDataManager.Instance.IsLoggedIn ? 
                RemoteDataManager.Instance.OurPlayFabId : 
                LeanLocalization.GetTranslationText(_notLoggedInLocalisationTerm, "[user not logged in]");
            RefreshFriendsListUI();
        }

        private void RefreshFriendsListUI()
        {
            _friendsListRoot.DestroyAllChildren();
            RemoteDataManager.Instance.FriendDisplayNames.ApplyFunction(p =>
                Instantiate(_friendEntryPrefab, _friendsListRoot).GetComponentInChildren<TextMeshProUGUI>().text = p);
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
                
                _friendsLevelScreen.RefreshScreen();
                RemoteDataManager.Instance.UpdateFriendsList(success =>
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