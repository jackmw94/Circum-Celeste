using UnityEngine;

namespace Code.Core
{
    public abstract class LevelElement : MonoBehaviour
    {
        public virtual void LevelStarted(){}
        public virtual void LevelFinished(){}
        public virtual void LevelReset(){}
    }
}