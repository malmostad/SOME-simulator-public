using System;

namespace SoMeSimulator.Data.Models
{
    [Flags]
    public enum SessionLogTag: uint
    {
        None = 1,
        Facts = 2,
        Emotional = 4,
        Tone = 8,
    }
}