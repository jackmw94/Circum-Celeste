using UnityEngine;

namespace Code.UI
{
    public class FriendsScreenSelectable : Selectable
    {
        public bool IsSelected { get; private set; }
        
        public override void SetOnOff(bool isOn, Color colour, float duration)
        {
            IsSelected = isOn;
        }
    }
}