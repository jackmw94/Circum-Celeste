using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Code.Debugging;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Code.Core
{
    public class ApplicationManager : MonoBehaviour
    {
        [SerializeField] private int _targetFrameRate = 60;
        [SerializeField] private bool _vsyncOn;

        private static bool _loggedException = false;
        
        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;
            QualitySettings.vSyncCount = _vsyncOn ? 1 : 0;

            Application.logMessageReceived += ExceptionListener;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= ExceptionListener;
        }

        private void ExceptionListener(string condition, string stackTrace, LogType logType)
        {
            if (logType != LogType.Exception)
            {
                return;
            }
            
            TryLogExceptionToPlayFab($"{condition}\n --- \n{stackTrace}");
        }

        private void TryLogExceptionToPlayFab(string exception)
        {
            _loggedException = true;
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {"Exception", exception}
                }
            }, result =>
            {
                CircumDebug.Log("Logged exception to playfab");
            }, error =>
            {
                CircumDebug.LogError("Could not save exception to playfab");
            });
        }
    }
}