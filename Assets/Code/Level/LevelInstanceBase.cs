using System;
using UnityEngine;

namespace Code.Level
{
    public abstract class LevelInstanceBase : MonoBehaviour
    {
        public virtual bool PlayerStartedPlaying => true;
        
        public abstract void LevelReady();
        public abstract void StartLevel(Action<LevelResult> levelFinishedCallback);
        public abstract Vector3 GetPlayerPosition(int playerIndex);
        
    }
}