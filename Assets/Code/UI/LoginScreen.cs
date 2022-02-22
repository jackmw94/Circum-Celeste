using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class LoginScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _usernameTaken;
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private Button _loginButton;

        private void Awake()
        {
            _usernameInputField.onDeselect.AddListener(username =>
            {
                if (string.IsNullOrEmpty(username))
                {
                    return;
                }
            });
        }
    }
}