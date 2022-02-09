﻿using System;
using Code.Core;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.UI
{
    [Serializable]
    public class PlayerLevelData
    {
        [SerializeField] private string _playfabId;
        [SerializeField] private string _username;
        [SerializeField] private float _levelTime;
        [SerializeField] private bool _isPerfect;

        public string PlayfabId => _playfabId;
        public string Username => _username;
        public float Time => _levelTime;
        public bool IsPerfect => _isPerfect;

        public bool IsOurRecord => PlayfabId.EqualsIgnoreCase(RemoteDataManager.Instance.OurPlayFabId);
    }
}