using Code.Core;
using PlayFab.ClientModels;

namespace Code.UI
{
    public class FriendsPlayerLevelRanking : PlayerLevelRankingPanel
    {
        protected override ExecuteCloudScriptRequest GetCloudScriptRequest() => new ExecuteCloudScriptRequest
        {
            FunctionName = "getBestLevels",
            FunctionParameter = new {levelDataKey = PersistentDataKeys.LevelMetaStats(_currentLevelName), titleDisplayName = RemoteDataManager.Instance.OurDisplayName}
        };
    }
}