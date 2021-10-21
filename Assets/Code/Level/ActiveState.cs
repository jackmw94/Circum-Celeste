using System;

namespace Code.Level
{
    public class ActiveState // todo: find a way to make this more generic
    {
        [Flags]
        public enum ActiveReason
        {
            None = 0,
            UserSetting = 1 << 0,
            Gameplay = 1 << 1,
            Visible = UserSetting | Gameplay
        }
        
        private ActiveReason _activeState = ActiveReason.None;

        public bool IsActive => _activeState == ActiveReason.Visible;

        public void SetUnsetReason(ActiveReason reason, bool set)
        {
            if (set)
            {
                _activeState |= reason;
            }
            else
            {
                _activeState &= ~reason;
            }
        }

        public bool IsActiveForReason(ActiveReason reason)
        {
            return _activeState.HasFlag(reason);
        }
    }
}