using System.Collections;
using System.Linq;
using Code.Core;
using UnityCommonFeatures;
using UnityEngine;

namespace Code.UI
{
    public class DebugSettingsButton : ButtonBehaviour
    {
        private string[] DebugPlayfabIds => new []
        {
            "A64215A754EDBDEA",
            "2804D9FF3B22DDBB"
        };

        [SerializeField] private GameObject _debugSettings;
        
        private IEnumerator Start()
        {
            _debugSettings.SetActive(false);
            
            yield return new WaitUntil(() => RemoteDataManager.Instance.IsLoggedIn);
            
            string ourPlayFabId = RemoteDataManager.Instance.OurPlayFabId;
            bool isADebugPlayer = DebugPlayfabIds.Any(p => p.Equals(ourPlayFabId));
            gameObject.SetActive(isADebugPlayer);
        }
        
        protected override void OnButtonClicked()
        {
            _debugSettings.SetActive(true);
        }
    }
}