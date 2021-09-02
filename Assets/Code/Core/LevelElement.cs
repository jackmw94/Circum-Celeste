using UnityEngine;

namespace Code.Core
{
    public abstract class LevelElement : MonoBehaviour
    {
        /// <summary>
        /// Called to setup the level as soon as it generates. Intended to not yet shown be to the user, still behind overlay
        /// </summary>
        public virtual void LevelSetup(){}

        /// <summary>
        /// Called once a level is ready for the player to start
        /// </summary>
        public virtual void LevelReady(){}

        /// <summary>
        /// Called once a player moves and the action starts
        /// </summary>
        public virtual void LevelStarted(){}

        /// <summary>
        /// Level finished is called when a level is EITHER completed or failed
        /// </summary>
        public virtual void LevelFinished(){}
    }
}