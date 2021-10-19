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
            PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest()
                {
                    Body = new Dictionary<string, object>()
                    {
                        {"LevelNumber", levelNumber},
                    },
                    EventName = "StartedLevel"
                },
                result =>
                {
                    CircumDebug.Log($"Wrote started level event for level number {levelNumber}");
                },
                error =>
                {
                    CircumDebug.LogError($"Could not log level started event! {error.GenerateErrorReport()}");
                });
        }
        
        public static void LevelCompletedEvent(int levelNumber, bool isPerfect, bool beatGoldTime)
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
            }, response =>
            {
                CircumDebug.Log($"Wrote completed level event for level number {levelNumber} (perf={isPerfect}, beatGold={beatGoldTime})");
            }, error =>
            {
                CircumDebug.LogError($"Could not log level completed event! {error.GenerateErrorReport()}");
            });
        }
    }
}