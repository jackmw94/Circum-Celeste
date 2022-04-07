using Code.Core;
using PlayFab;
using UnityEngine;
#if ENABLE_PLAYFABADMIN_API
using PlayFab.AdminModels;
#endif

namespace Code.Level
{
    [CreateAssetMenu(menuName = "Create ChallengeLevelUploaderTool", fileName = "ChallengeLevelUploaderTool", order = 0)]
    public class ChallengeLevelUploaderTool : ScriptableObject
    {
        [SerializeField] private ChallengeLevel[] _challengeLevel;

        [ContextMenu(nameof(SetChallengeTitleData))]
        private void SetChallengeTitleData()
        {
#if ENABLE_PLAYFABADMIN_API
            foreach (ChallengeLevel challengeLevel in _challengeLevel)
            {
                string titleDataKey = PersistentDataKeys.ChallengeName(challengeLevel.MonthIndex);

                ChallengeLevel.ChallengeLevelProxy challengeLevelProxy = challengeLevel.GetChallengeLevelProxy();
                string serializedChallengeLevelProxy = JsonUtility.ToJson(challengeLevelProxy);

                PlayFabAdminAPI.SetTitleData(new SetTitleDataRequest
                    {
                        Key = titleDataKey,
                        Value = serializedChallengeLevelProxy
                    }, 
                    result =>
                    {
                        Debug.Log($"Set title data for {challengeLevel.LevelName} at month index: {challengeLevel.MonthIndex}. {titleDataKey}");
                    }, error => { Debug.LogError(error.ErrorMessage); });
            }
#else
            Debug.LogError("Add ENABLE_PLAYFABADMIN_API to project symbols!");
#endif
        }
    }
}