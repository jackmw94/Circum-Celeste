using Code.Core;
using Code.Debugging;
using Code.Level;
using UnityEngine;

namespace Code.Flow
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private InterLevelFlow _interLevelFlow;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private LevelProvider _levelProvider;

        private bool _hasRemoteConfig = false;

        private void Start()
        {
            _interLevelFlow.ShowInterLevelUI(true);
            
            RemoteConfigHelper.RequestRefresh(success =>
            {
                CircumDebug.Log("Remote config request finished, showing level");
                
                // true even if request failed, we'll have reverted to backup data and logged the error
                _hasRemoteConfig = true;
                
                // hide splash screen now
            });
        }
    }
}