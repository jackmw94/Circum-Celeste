using System;
using UnityEngine;

namespace Code.UI
{
    [Serializable]
    public class PlayerLevelsData
    {
        [SerializeField] private int _totalNumberOfFriends;
        [SerializeField] private PlayerLevelData[] _data;

        public int TotalNumberOfFriends => _totalNumberOfFriends;
        public PlayerLevelData[] Data => _data;
    }
}