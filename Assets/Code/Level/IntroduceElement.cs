using System;

namespace Code.Level
{
    [Flags]
    public enum IntroduceElement
    {
        None = 0,
        PowerButton = 1 << 0,
        MovementHandle = 1 << 1,
        Escape = 1 << 2,
        Orbiter = 1 << 3,
    }
}