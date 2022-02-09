using System;
using UnityEngine;

namespace Code.Core
{
    [CreateAssetMenu(menuName = "Create PlayerProvider", fileName = "PlayerProvider", order = 0)]
    public class PlayerProvider : ScriptableObject
    {
        public enum PlayerType
        {
            Regular,
            Newtonian
        }

        [SerializeField] private GameObject _regularPlayer;
        [SerializeField] private GameObject _newtonianPlayer;

        public GameObject GetPlayer(PlayerType playerType)
        {
            switch (playerType)
            {
                case PlayerType.Regular: return _regularPlayer;
                case PlayerType.Newtonian: return _newtonianPlayer;
                default: throw new ArgumentOutOfRangeException($"No player for player type {playerType}");
            }
        }
    }
}