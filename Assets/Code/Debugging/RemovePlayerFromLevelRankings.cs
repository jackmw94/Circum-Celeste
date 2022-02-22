using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Code.Debugging
{
    public class RemovePlayerFromLevelRankings : MonoBehaviour
    {
        // default value is for jmw2g12 account
        [SerializeField] private string _playerId = "2804D9FF3B22DDBB";
        
        [ContextMenu(nameof(RemovePlayerFromRankings))]
        private void RemovePlayerFromRankings()
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                CustomId = "Lerpz_1000"
            }, result =>
            {
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
                {
                    FunctionName = "removePlayerFromGlobalRanking",
                    FunctionParameter = new {PlayerId = _playerId}
                }, Debug.Log, Debug.Log);
            }, Debug.LogError);
        }
    }
}