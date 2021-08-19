using UnityEngine;

namespace Code.Core
{
    public abstract class LevelElement : MonoBehaviour
    {
        /// <summary>
        /// Called once a player moves and the action starts
        /// </summary>
        public virtual void LevelStarted(){}
        
        /// <summary>
        /// Level finished is called when a level is EITHER completed or failed
        /// </summary>
        public virtual void LevelFinished(){}
        
        /// <summary>
        /// Called to setup the level ahead of it starting. Note this could be for as long as input is withheld, not just a few frames
        /// </summary>
        public virtual void LevelReset(){}
    }
}