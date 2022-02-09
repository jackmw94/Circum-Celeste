using Code.Core;
using UnityEngine;

namespace Code.Level
{
    public class Orbiter : LevelElement
    {
        // level element so it's tracked by recorder
        // level state functions are not called

        [SerializeField] private OrbiterMover _mover;
        
        public void SetupGridSize(int gridSize)
        {
            _mover.SetupGridSize(gridSize);
        }
    }
}