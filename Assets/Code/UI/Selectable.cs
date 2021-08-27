using UnityEngine;

namespace Code.UI
{
    public abstract class Selectable : MonoBehaviour
    {
        public abstract void SetOnOff(bool isOn, Color colour, float duration);
    }
}