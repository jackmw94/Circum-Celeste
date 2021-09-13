using Code.Core;
using Code.Level.Player;
using UnityEngine;

namespace Code.Level
{
    public class Orbiter : LevelElement
    {
        // level element so it's tracked by recorder
        // level state functions are not called

        [SerializeField] private OrbiterMover _mover;

        public void SetPidValues(Vector3 pidValues)
        {
            _mover.SetPidValues(pidValues);
        }
    }
}