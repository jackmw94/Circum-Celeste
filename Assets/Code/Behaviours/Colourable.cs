using UnityEngine;

namespace Code.Behaviours
{
    public abstract class Colourable : MonoBehaviour
    {
        public abstract Color GetColour();
        public abstract void SetColour(Color colour);
    }
}