﻿using System;
using Code.Core;
using PlayFab;
using PlayFab.AdminModels;
using UnityEngine;

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
                string titleDataKey = PersistentDataKeys.ChallengeName(challengeLevel.WeekIndex);

                ChallengeLevel.ChallengeLevelProxy challengeLevelProxy = challengeLevel.GetChallengeLevelProxy();
                string serializedChallengeLevelProxy = JsonUtility.ToJson(challengeLevelProxy);

                PlayFabAdminAPI.SetTitleData(new SetTitleDataRequest
                    {
                        Key = titleDataKey,
                        Value = serializedChallengeLevelProxy
                    }, 
                    result =>
                    {
                        Debug.Log($"Set title data for {challengeLevel.LevelName} at week {challengeLevel.WeekIndex}");
                    }, error => { Debug.LogError(error.ErrorMessage); });
            }
#else
            CircumDebug.LogError("Add ENABLE_PLAYFABADMIN_API to project symbols!");
#endif
        }
    }
}