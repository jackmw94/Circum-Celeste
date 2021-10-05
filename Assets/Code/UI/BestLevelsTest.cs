using Code.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class BestLevelsTest : MonoBehaviour
{
    [ContextMenu(nameof(AddFriend))]
    private void AddFriend()
    {
        PlayFabClientAPI.AddFriend(new AddFriendRequest()
        {
            FriendPlayFabId = "2A4F4DB10790E21A"
        }, result =>
        {
            Debug.Log($"Result: {result}");
        }, error =>
        {
            Debug.LogError($"Error: {error.ErrorMessage}");
        });
    }
    
    public void Run()
    {
        Debug.Log("Running...");
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        {
            CustomId = "Lerpz_1000",
        }, result =>
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName = "getBestLevels",
                FunctionParameter = new { levelDataKey = "Circum_LevelStats_Two Moons" }
            }, scriptResult =>
            {
                Debug.Log("Returned from get best level");
                scriptResult.Logs.ForEach(p =>
                {
                    Debug.Log(p.Message);
                });
                Debug.Log(scriptResult.FunctionResult);

                string serialized = scriptResult.FunctionResult.ToString();
                FriendsLevelRanking.FriendLevelsData deserialized = JsonUtility.FromJson<FriendsLevelRanking.FriendLevelsData>(serialized);
                
                
                if (scriptResult.Error != null)
                {
                    Debug.LogError($"{scriptResult.Error.Error} : {scriptResult.Error.Message}\n{scriptResult.Error.StackTrace}");
                }
            }, error =>
            {
                Debug.LogError(error.ErrorMessage);
            });
        }, error =>
        {
            Debug.LogError(error.ErrorMessage);
        });
    }
}