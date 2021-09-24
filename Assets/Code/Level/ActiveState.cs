using System;

namespace Code.Level
{
    public class ActiveState
    {
        [Flags]
        public enum ActiveReason
        {
            None = 0,
            UserSetting = 1 << 0,
            Gameplay = 1 << 1,
            Visible = UserSetting | Gameplay
        }
        
        private ActiveReason _optionalUIVisibility = ActiveReason.None;

        public bool IsActive => _optionalUIVisibility == ActiveReason.Visible;

        public void SetUnsetReason(ActiveReason reason, bool set)
        {
            if (set)
            {
                _optionalUIVisibility |= reason;
            }
            else
            {
                _optionalUIVisibility &= ~reason;
            }
        }
    }
}