using System.Diagnostics;
using Code.Core;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class ShareFriendCodeButton : MonoBehaviour
    {
        [SerializeField] private Button _shareButton;
        [SerializeField, LeanTranslationName] private string _addMeAsFriendLocalisationTerm;

        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (!_shareButton)
            {
                _shareButton = GetComponent<Button>();
            }
        }

        private void Awake()
        {
            _shareButton.onClick.AddListener(ShareButtonListener);
        }

        private void OnEnable()
        {
            _shareButton.interactable = RemoteDataManager.Instance.IsLoggedIn;
        }

        private void OnDestroy()
        {
            _shareButton.onClick.RemoveListener(ShareButtonListener);
        }

        private void ShareButtonListener()
        {
            NativeShare nativeShare = new NativeShare();
            string uniqueId = RemoteDataManager.Instance.OurPlayFabId;
            string localisedMessage = LeanLocalization.GetTranslationText(_addMeAsFriendLocalisationTerm, "Add me as a friend on Circum Celeste! My friend ID is: {0}");
            string formattedMessage = string.Format(localisedMessage, uniqueId);
            nativeShare.SetText(formattedMessage);
            nativeShare.SetUrl("https://bit.ly/circum_ios");
            nativeShare.Share();
        }
    }
}