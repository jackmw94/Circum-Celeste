using System.Collections.Generic;
using Code.Debugging;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Code.Core
{
    public static class AnalyticsHelper
    {
        public static void LevelStartedEvent(int levelNumber)
        {
            if (!RemoteDataManager.Instance.IsLoggedIn)
            {
                return;
            }

            try
            {
                PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest()
                    {
                        Body = new Dictionary<string, object>()
                        {
                            {"LevelNumber", levelNumber},
                        },
                        EventName = "StartedLevel"
                    },
                    result => { CircumDebug.Log($"Wrote started level event for level number {levelNumber}"); },
                    error => { CircumDebug.LogError($"Could not log level started event! {error.GenerateErrorReport()}"); });
            }
            catch (PlayFabException playFabException)
            {
                CircumDebug.LogError($"Could not send analytics event : {playFabException.Message}");
            }
        }
        
        public static void LevelCompletedEvent(int levelNumber, bool isPerfect, bool beatGoldTime)
        {
            if (!RemoteDataManager.Instance.IsLoggedIn)
            {
                return;
            }

            try
            {
                PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest()
                    {
                        Body = new Dictionary<string, object>()
                        {
                            {"LevelNumber", levelNumber},
                            {"NoDamage", isPerfect},
                            {"BeatGoldTime", beatGoldTime}
                        },
                        EventName = "CompletedLevel"
                    }, response => { CircumDebug.Log($"Wrote completed level event for level number {levelNumber} (perf={isPerfect}, beatGold={beatGoldTime})"); },
                    error => { CircumDebug.LogError($"Could not log level completed event! {error.GenerateErrorReport()}"); });
            }
            catch (PlayFabException playFabException)
            {
                CircumDebug.LogError($"Could not write analytics event : {playFabException.Message}");
            }
        }
    }
}