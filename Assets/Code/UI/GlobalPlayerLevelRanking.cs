using PlayFab.ClientModels;

namespace Code.UI
{
    public class GlobalPlayerLevelRanking : PlayerLevelRankingPanel
    {
        protected override ExecuteCloudScriptRequest GetCloudScriptRequest() => new ExecuteCloudScriptRequest
        {
            FunctionName = "getGlobalRankingsForLevel",
            FunctionParameter = new {LevelId = _currentLevelName}
        };
    }
}