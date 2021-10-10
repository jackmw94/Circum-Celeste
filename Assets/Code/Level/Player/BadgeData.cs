using System;

namespace Code.Level.Player
{
    public struct BadgeData
    {
        // todo: change from bools to badge state; fewer variables and no invalid states
        public BadgeState BadgeState { get; set; }
        public bool IsPerfect { get; set; }
        public bool HasGoldTime { get; set; }
        public bool HasPerfectGoldTime { get; set; }
    }

    [Flags]
    public enum BadgeState
    {
        None = 0,
        Perfect = 1 << 0,
        GoldTime = 1 << 1,
        PerfectGoldTime = Perfect | GoldTime
    }
}