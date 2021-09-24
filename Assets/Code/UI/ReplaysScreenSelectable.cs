using Code.Level.Player;
using UnityEngine;

namespace Code.UI
{
    public class ReplaysScreenSelectable : Selectable
    {
        // sometimes this gets selected during initialisation
        // shouldn't ever get intentionally selected until after startup
        private const float StartDelay = 3f;
        
        private PersistentDataManager _persistentDataManager;
        
        private void Awake()
        {
            _persistentDataManager = PersistentDataManager.Instance;
        }

        public override void SetOnOff(bool isOn, Color colour, float duration)
        {
            if (Time.time < StartDelay)
            {
                return;
            }
            
            if (!isOn)
            {
                return;
            }

            if (_persistentDataManager.PlayerFirsts.SeenReplaysScreen)
            {
                return;
            }
            
            _persistentDataManager.PlayerFirsts.SetReplaysScreenAsSeen();
        }
    }
}