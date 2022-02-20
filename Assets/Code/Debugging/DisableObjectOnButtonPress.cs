using UnityCommonFeatures;
using UnityEngine;

namespace Code.Debugging
{
    public class DisableObjectOnButtonPress : ButtonBehaviour
    {
        [SerializeField] private GameObject _disableObject;
        
        protected override void OnButtonClicked()
        {
            _disableObject.SetActive(false);
        }
    }
}